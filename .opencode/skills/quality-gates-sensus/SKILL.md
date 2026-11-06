---
name: quality-gates-sensus
description: Gates locales y CI para Sensus Backend: build, formato, cobertura, Snyk, SonarCloud, SDD, arquitectura
license: MIT
---

# Quality Gates — Sensus Backend

## Objetivo
Garantizar que cada cambio cumple con `constitution.md` v1.2.0 (Principio V — Verificación automatizada y Principio IV — Seguridad) y `AGENTS.md` §6 y §11.

## Gates locales (ejecutar antes de entregar)

| Gate | Comando | Umbral / Resultado |
|------|---------|-------------------|
| Compilación | `dotnet build sensus-back.sln --configuration Release -warnaserror` | 0 errores, 0 warnings |
| Formato | `dotnet format sensus-back.sln --verify-no-changes` | Sin cambios pendientes |
| Pruebas unitarias + arquitectura | `dotnet test sensus-back.sln --configuration Release` | Todas verdes |
| Cobertura (Domain + Application) | `dotnet test sensus-back.sln --collect:"XPlat Code Coverage"` + `node .github/scripts/check-coverage.mjs artifacts/coverage 80 Sensus.Domain Sensus.Application` | ≥ 80 % líneas |
| Estructura SDD | `node .github/scripts/check-specs.mjs specs` | 44 specs, 4 artefactos cada una |
| Arquitectura | `dotnet test tests/Sensus.ArchitectureTests` | En verde |

## Gates CI (`.github/workflows/ci.yml`)

| Job | Propósito | Bloqueante |
|-----|-----------|------------|
| `build-and-test` | Compilación, formato, unitarias, integración (MongoDB real), cobertura ≥80% | SÍ |
| `snyk-security` | Snyk test dependencias (severity ≥ high) + Snyk monitor | SÍ |
| `sonarcloud` | SonarCloud análisis estático + calidad gate | SÍ |
| `quality` | `dotnet list package --vulnerable`, gitleaks (bloqueante), NetArchTest | SÍ |
| `iac` | `cdk synth` + `cdk diff` (si existe `infra/`) | SÍ (en PR) |
| `sdd` | `check-specs.mjs specs` — 4 artefactos por spec | SÍ |

## Reglas críticas (Constitución §225, §196, §188)

- **Nunca** usar `continue-on-error: true` en gates de seguridad o cobertura.
- **Gitleaks** debe fallar si encuentra secretos (sin `continue-on-error`).
- **Snyk** debe fallar si hay vulnerabilidades `high` o `critical`.
- **SonarCloud** calidad gate debe pasar antes del merge.
- **Cobertura** ≥ 80% en `Sensus.Domain` y `Sensus.Application` es umbral bloqueante.
- **Pruebas de integración** usan MongoDB real vía Testcontainers, nunca dobles del motor.

## Secretos en CI (nunca en código)

| Secreto | GitHub Secret | Uso |
|---------|--------------|-----|
| `SNYK_TOKEN` | `secrets.SNYK_TOKEN` | Autenticación Snyk CLI |
| `SONAR_TOKEN` | `secrets.SONAR_TOKEN` | Análisis SonarCloud |
| `SONAR_ORGANIZATION` | `secrets.SONAR_ORGANIZATION` | Organización SonarCloud |
| `GITHUB_TOKEN` | `secrets.GITHUB_TOKEN` | Gitleaks (proporcionado por GitHub) |

## Anti-patrones

- Bajar umbral de cobertura por debajo del 80%.
- Marcar pruebas como omitidas (`[Fact(Skip=...)]`) sin justificación aprobada.
- Fusionar con CI en rojo.
- Escribir tokens o secretos en el workflow YAML.
- Usar dobles que simulen MongoDB en pruebas de persistencia.
