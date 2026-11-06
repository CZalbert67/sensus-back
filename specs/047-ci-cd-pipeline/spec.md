# Spec 047 — Pipeline CI-CD y calidad de software (DevSecOps)

| Campo | Valor |
|---|---|
| ID | 047 |
| Slug | `ci-cd-pipeline` |
| Estado | Pendiente |
| Ola | 7 |
| Tipo | Nuevo |
| Dueño | Por asignar |
| Dependencias | 009, 010, 011, 012, 013 |
| Versión | 1.0.0 |
| Alineado a | `constitution.md` v1.2.0 · `AGENTS.md` v1.2.0 |

---

## 1. Contexto y problema

El backend Sensus requiere un pipeline de integración continua y despliegue continuo (CI-CD) que ejecute, como mínimo: compilación, pruebas unitarias, pruebas de integración con base de datos real, pruebas de seguridad (Snyk), análisis de código (SonarCloud), pruebas de mutación (Stryker.NET), escaneo de secretos (gitleaks), validación de arquitectura y cobertura de pruebas con umbral bloqueante. Actualmente el pipeline está escrito pero no ejecuta pruebas de integración, no incluye Snyk, no incluye SonarCloud, no incluye mutación y tiene gates con `continue-on-error: true` que violan el principio de bloqueo de fusiones.

## 2. Alcance

**Incluye**

- Pipeline GitHub Actions con jobs: `build-and-test`, `integration-tests`, `snyk-security`, `mutation-testing`, `sonarcloud`, `quality`, `iac`, `sdd`
- Pruebas de integración contra MongoDB real vía Testcontainers en job aislado
- Snyk: escaneo de dependencias (bloqueante, severidad ≥ high) y monitor
- SonarCloud: análisis estático, calidad gate, cobertura publicada
- Stryker.NET: pruebas de mutación (advisory, no bloqueante)
- Gitleaks: escaneo de secretos (bloqueante)
- Coverlet: cobertura ≥ 80 % en Domain y Application (bloqueante)
- `dotnet format --verify-no-changes` (bloqueante)
- `check-specs.mjs` para trazabilidad SDD (bloqueante)
- `cdk synth` y `cdk diff` para IaC (si existe `infra/`)
- Configuración de secretos únicamente en GitHub Secrets (`SNYK_TOKEN`, `SONAR_TOKEN`, `SONAR_ORGANIZATION`)

**No incluye**

- `cdk deploy` (requiere autorización explícita, constitution.md §6)
- Despliegue a instancias EC2 (constitution.md §10: cómputo exclusivamente Lambda)
- Terraform (constitution.md §10: IaC con AWS CDK en C#)
- UI o frontend (fuera del alcance del backend, constitution.md §2)

## 3. Actores

- Desarrollador (ejecuta gates locales antes de push)
- Revisor de PR (valida que pipeline esté en verde)
- Operador de CI/CD (mantiene secrets y runners)

## 4. Requisitos funcionales

| ID | Requisito | Prioridad |
|---|---|---|
| FR-047-01 | El pipeline compila la solución con `-warnaserror` (0 warnings, 0 errores) | Must |
| FR-047-02 | Ejecuta pruebas unitarias sin integración en job aislado | Must |
| FR-047-03 | Ejecuta pruebas de integración contra MongoDB real en job separado con servicio Docker | Must |
| FR-047-04 | Ejecuta Snyk `test` con severidad ≥ high como paso bloqueante | Must |
| FR-047-05 | Ejecuta SonarCloud análisis con calidad gate bloqueante | Must |
| FR-047-06 | Ejecuta Stryker.NET pruebas de mutación como paso advisory | Should |
| FR-047-07 | Ejecuta gitleaks escaneo de secretos como paso bloqueante | Must |
| FR-047-08 | Verifica cobertura ≥ 80 % en `Sensus.Domain` y `Sensus.Application` como paso bloqueante | Must |
| FR-047-09 | Ejecuta `dotnet format --verify-no-changes` como paso bloqueante | Must |
| FR-047-10 | Ejecuta `check-specs.mjs specs` para validar 4 artefactos por spec | Must |
| FR-047-11 | Ejecuta `cdk synth` y `cdk diff` en PR cuando exista `infra/` | Should |
| FR-047-12 | Los secretos (`SNYK_TOKEN`, `SONAR_TOKEN`) solo como GitHub Secrets, nunca en YAML | Must |

## 5. Requisitos no funcionales

| ID | Requisito | Métrica objetivo |
|---|---|---|
| NFR-047-01 | Tiempo total del pipeline | < 10 minutos |
| NFR-047-02 | Cobertura de pruebas | ≥ 80 % líneas en Domain y Application |
| NFR-047-03 | Vulnerabilidades críticas/altas en dependencias | 0 |
| NFR-047-04 | Secretos en código o YAML del workflow | 0 |
| NFR-047-05 | Fallo de cualquier verificación bloqueante | Bloquea fusión (merge) |

## 6. Criterios de aceptación

### CA-047-01: Pipeline compila limpio
- **Dado**: un push o PR a `main` o `Sprint1`
- **Cuando**: se ejecuta el job `build-and-test`
- **Entonces**: `dotnet build -warnaserror` finaliza con 0 errores y 0 warnings

### CA-047-02: Pruebas unitarias y de arquitectura
- **Dado**: cambios en el código
- **Cuando**: se ejecutan pruebas unitarias y de arquitectura
- **Entonces**: todas las pruebas están en verde

### CA-047-03: Pruebas de integración con MongoDB real
- **Dado**: un PR con cambios que afectan persistencia
- **Cuando**: se ejecuta el job `integration-tests` con servicio `mongo:7.0`
- **Entonces**: las pruebas de integración pasan contra MongoDB real vía Testcontainers

### CA-047-04: Snyk bloqueante
- **Dado**: una dependencia con vulnerabilidad severidad high o crítica
- **Cuando**: se ejecuta `snyk test --severity-threshold=high`
- **Entonces**: el job falla y bloquea el merge

### CA-047-05: SonarCloud calidad gate
- **Dado**: código con code smells o bugs
- **Cuando**: se ejecuta el análisis de SonarCloud
- **Entonces**: la calidad gate debe pasar antes de permitir el merge

### CA-047-06: Cobertura bloqueante
- **Dado**: cobertura medida por Coverlet
- **Cuando**: `check-coverage.mjs` evalúa Domain y Application
- **Entonces**: si es < 80 %, el job falla y bloquea el merge

### CA-047-07: Sin `continue-on-error` en gates críticos
- **Dado**: el archivo `.github/workflows/ci.yml`
- **Cuando**: se inspecciona cada job
- **Entonces**: ningún paso de seguridad, cobertura, formato, SDD o arquitectura usa `continue-on-error: true`

### CA-047-08: Secretos fuera del código
- **Dado**: el workflow YAML
- **Cuando**: se revisan los valores de autenticación
- **Entonces**: `SNYK_TOKEN` y `SONAR_TOKEN` se leen solo de `${{ secrets.* }}`, nunca hardcodeados

---

## 7. Contratos

No expone endpoints públicos; es un pipeline de infraestructura de desarrollo.

## 8. Exclusiones

- No incluye despliegue a producción (`cdk deploy` requiere autorización explícita).
- No incluye Terraform (constitution.md §10 usa AWS CDK en C#).
- No incluye frontend ni aplicación móvil.

---
