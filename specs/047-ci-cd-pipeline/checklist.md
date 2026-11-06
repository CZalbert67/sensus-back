# Checklist 047 — Pipeline CI-CD y calidad de software (DevSecOps)

Alineado a `constitution.md` v1.2.0 · `AGENTS.md` v1.2.0.

Usar antes, durante y al cierre de la implementación. Ninguna tarea se cierra con casillas pendientes.

## A. Antes de empezar (Definition of Ready)

- [ ] La spec 047 está aprobada y sin preguntas bloqueantes
- [ ] Los requisitos tienen identificador y criterio medible
- [ ] Las dependencias están resueltas: 009, 010, 011, 012, 013
- [ ] El alcance y las exclusiones están declarados
- [ ] El impacto en seguridad, costo y rendimiento fue evaluado

## B. Específicos de este spec

- [ ] El job `build-and-test` no tiene `services.mongo`
- [ ] El job `integration-tests` tiene `services.mongo` con `mongo:7.0` y healthcheck
- [ ] El job `snyk-security` ejecuta `snyk test --severity-threshold=high` sin `continue-on-error`
- [ ] El job `sonarcloud` ejecuta análisis con calidad gate bloqueante
- [ ] El job `mutation-testing` ejecuta `dotnet-stryker` con `continue-on-error: true` (advisory)
- [ ] El job `quality` ejecuta gitleaks sin `continue-on-error`
- [ ] El job `sdd` ejecuta `check-specs.mjs specs`
- [ ] Los secretos `SNYK_TOKEN` y `SONAR_TOKEN` se leen de `${{ secrets.* }}`
- `stryker-config.json` existe en la raíz del repositorio

## C. Antes de entregar (Definition of Done)

- [ ] Código en la capa correcta y test de arquitectura en verde
- [ ] Pruebas unitarias y de integración añadidas y en verde
- [ ] Cobertura ≥ 80 % en Domain y Application
- [ ] Contrato OpenAPI actualizado si cambió la API
- [ ] IaC actualizada y `cdk diff` revisado si aplica
- [ ] Logs con correlation-id y sin PII
- [ ] Revisión de seguridad: sin secretos, autorización verificada
- [ ] Constitution Check en PASS o brecha registrada

## D. Constitution Check

- [ ] **I. Especificación y trazabilidad** — PASS
- [ ] **II. Clean Architecture** — PASS
- [ ] **III. CQRS y contratos** — PASS
- [ ] **IV. Seguridad y privacidad** — PASS
- [ ] **V. Verificación automatizada** — PASS
- [ ] **VI. Costo cero e IaC** — PASS
- [ ] **VII. Rendimiento y observabilidad** — PASS

---
