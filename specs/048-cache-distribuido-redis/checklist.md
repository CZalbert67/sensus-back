# Checklist 048 — Caché distribuido con Redis

Alineado a `constitution.md` v1.2.0 · `AGENTS.md` v1.2.0.

Usar antes, durante y al cierre de la implementación. Ninguna tarea se cierra con casillas pendientes.

## A. Antes de empezar (Definition of Ready)

- [ ] La spec 048 está aprobada y sin preguntas bloqueantes
- [ ] Los requisitos tienen identificador y criterio medible
- [ ] Las dependencias están resueltas: 010, 024, 026, 032
- [ ] El alcance y las exclusiones están declarados
- [ ] El impacto en seguridad, costo y rendimiento fue evaluado

## B. Específicos de este spec

- [ ] Existe interfaz `ICacheService` en `Sensus.Application.Common.Interfaces`
- [ ] Existe implementación `RedisCacheService` en `Sensus.Infrastructure`
- [ ] `StackExchange.Redis` solo referenciado en `Sensus.Infrastructure`
- [ ] `ICacheService` registrado en DI de `Sensus.Api/Program.cs`
- [ ] TTL configurado en `appsettings.json` (no hardcodeado)
- [ ] Caché integrado en `GET /offers/{id}`, `GET /offers/nearby`, `GET /feed/nearby`, `GET /merchants/me`, `GET /users/profile`
- [ ] Invalidación al crear/actualizar/eliminar oferta
- [ ] Invalidación al crear/actualizar/eliminar publicación
- [ ] Invalidación al actualizar perfil de comerciante
- [ ] Fallback graceful a MongoDB si Redis no está disponible
- [ ] Health check del caché en `/health/ready`
- [ ] Sin PII ni tokens almacenados en caché

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
