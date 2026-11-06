#!/usr/bin/env node
/**
 * Verifica el umbral de cobertura por ensamblado (constitution.md P-V).
 *
 * Uso:
 *   node .github/scripts/check-coverage.mjs <directorio> <umbral> [ensamblados...]
 *
 * Lee los reportes Cobertura generados por `dotnet test --collect:"XPlat Code Coverage"`
 * y falla si alguno de los ensamblados indicados queda por debajo del umbral de líneas.
 *
 * Varios proyectos de prueba pueden generar un reporte cada uno, y todos instrumentan los
 * mismos ensamblados de producción. Por eso los reportes se **fusionan**: cada línea de código
 * (identificada por ensamblado + clase + número de línea) se cuenta una sola vez y se marca
 * como cubierta si cualquier reporte la ejecutó. Sumar los reportes sin fusionar inflaba el
 * denominador y hundía artificialmente el porcentaje.
 */
import fs from 'node:fs';
import path from 'node:path';

const DEFAULT_ASSEMBLIES = ['Sensus.Domain', 'Sensus.Application'];

function walk(dir) {
  const found = [];
  if (!fs.existsSync(dir)) return found;
  for (const entry of fs.readdirSync(dir, { withFileTypes: true })) {
    const full = path.join(dir, entry.name);
    if (entry.isDirectory()) found.push(...walk(full));
    else if (entry.name.endsWith('.cobertura.xml')) found.push(full);
  }
  return found;
}

/**
 * Extrae de un reporte Cobertura las líneas de los ensamblados objetivo.
 * Devuelve un Map con clave `ensamblado|clase|númeroDeLínea` y el máximo de hits observado.
 */
function collectLineHits(xml, targets) {
  const lines = new Map();
  const packageRegex = /<package\b([^>]*)>([\s\S]*?)<\/package>/g;

  for (const [, attributes, body] of xml.matchAll(packageRegex)) {
    const name = /name="([^"]*)"/.exec(attributes)?.[1] ?? '';
    const owner = targets.find((target) => name.includes(target));
    if (!owner) continue;

    const classRegex = /<class\b([^>]*)>([\s\S]*?)<\/class>/g;
    for (const [, classAttributes, classBody] of body.matchAll(classRegex)) {
      const className = /name="([^"]*)"/.exec(classAttributes)?.[1] ?? '';
      if (!className) continue;

      const lineRegex = /<line\b([^>]*)\/?>/g;
      for (const [, lineAttributes] of classBody.matchAll(lineRegex)) {
        const number = /number="(\d+)"/.exec(lineAttributes)?.[1];
        if (number === undefined) continue;
        const hits = Number(/hits="(\d+)"/.exec(lineAttributes)?.[1] ?? '0');
        const key = `${owner}|${className}|${number}`;
        const previous = lines.get(key);
        if (previous === undefined || hits > previous) lines.set(key, hits);
      }
    }
  }

  return lines;
}

const [root, rawThreshold, ...rest] = process.argv.slice(2);
if (!root || !rawThreshold) {
  console.error(
    'Uso: node .github/scripts/check-coverage.mjs <directorio> <umbral> [ensamblados...]',
  );
  process.exit(2);
}

const threshold = Number(rawThreshold);
const targets = rest.length > 0 ? rest : DEFAULT_ASSEMBLIES;
const files = walk(root);

if (files.length === 0) {
  console.error(`ERROR: no se encontraron reportes de cobertura en ${root}`);
  process.exit(1);
}

// Fusión global: una entrada por línea de código, con el máximo de hits entre reportes.
const merged = new Map();
for (const file of files) {
  const hits = collectLineHits(fs.readFileSync(file, 'utf8'), targets);
  for (const [key, value] of hits) {
    const previous = merged.get(key);
    if (previous === undefined || value > previous) merged.set(key, value);
  }
}

const total = new Map(targets.map((name) => [name, { covered: 0, valid: 0 }]));
for (const [key, hits] of merged) {
  const owner = key.slice(0, key.indexOf('|'));
  const entry = total.get(owner);
  if (!entry) continue;
  entry.valid += 1;
  if (hits > 0) entry.covered += 1;
}

let failed = false;
for (const name of targets) {
  const { covered, valid } = total.get(name);
  if (valid === 0) {
    console.error(`FALLO  ${name}: sin datos de cobertura`);
    failed = true;
    continue;
  }
  const pct = (covered / valid) * 100;
  const status = pct >= threshold ? 'OK    ' : 'FALLO ';
  console.log(
    `${status}${name}: ${pct.toFixed(2)} % (${Math.round(covered)}/${valid} líneas) — umbral ${threshold} %`,
  );
  failed = failed || pct < threshold;
}

process.exit(failed ? 1 : 0);
