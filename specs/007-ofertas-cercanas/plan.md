# Plan 007 — Ofertas cercanas

| Campo | Valor |
|---|---|
| Spec | [`spec.md`](spec.md) |
| Estado del plan | Aprobado |
| Versión | 1.1.0 |
| Constitution Check | PASS CON OBSERVACIONES |
| Alineado a | `constitution.md` v1.2.0 · `AGENTS.md` v1.2.0 |

> **v1.1.0** — Se aclara el uso de `$geoNear` para devolver la distancia, se registra que la
> colección `offers` depende de la spec 034 y se añaden las tareas documentales de alineación con el
> backlog de la escuela (T007-11 a T007-13).

## 1. Resumen técnico

Permitir que un usuario descubra ofertas activas cerca de su ubicación, con filtros por radio y categoría, resultados paginados y ordenados por distancia.

## 2. Cambios por capa

| Capa | Cambio |
|---|---|
| Application | GetNearbyOffersQuery, validador y proyección a DTO |
| Infrastructure | Consulta geoespacial con índice 2dsphere y filtro de estado |
| Api | Endpoint GET /api/v1/offers/nearby |

## 3. Modelo de datos e índices

- offers con location 2dsphere, status, stock, startsAt y endsAt; índice compuesto status + endsAt
- `location` se modela como `GeoJsonPoint<GeoJson2DGeographicCoordinates>` (`{ type: "Point", coordinates: [lng, lat] }`).
- La colección `offers` **no existe todavía**: su entidad, documento e índice se crean en la spec 034
  (creación de oferta). Este plan depende de esa spec además de 005 y 010.
- Para devolver la distancia (`FR-007-07`) se usa una **agregación `$geoNear` con `distanceField`**;
  `NearSphere` solo filtra y no expone la distancia. Equivalencia con `STDistance()` de SQL Server
  documentada en `spec.md` §8.

## 4. Seguridad

- Solo ofertas visibles públicamente
- Sin exponer dirección exacta del comercio
- Radio y paginación acotados

## 5. Observabilidad y rendimiento

- Métricas de latencia por consulta geoespacial
- Log del radio y pageSize solicitados

## 6. Costo AWS

Sin costo incremental: Lambda y API Gateway dentro del Free Tier.

## 7. Estrategia de pruebas

- Integración de paginación y límites
- Prueba de orden por distancia
- Prueba de exclusión por estado y vigencia
- Prueba de radio: aceptar 1–5 km para registrado (`FR-007-08`) y limitar a 2 km para invitado (`FR-007-09`)
- Prueba de que la respuesta incluye la distancia en metros (`$geoNear` + `distanceField`)

## 8. Constitution Check

Evaluación previa a la implementación.

| Principio | Decisión | Evidencia |
|---|---|---|
| I | PASS | `FR-007-01` y 6 requisitos más, con criterios de aceptación medibles |
| II | PASS | Cambios acotados a las capas declaradas en §2 |
| III | PASS | DTOs, comandos y consultas separados; sin exponer entidades de dominio |
| IV | PASS | Reglas de seguridad aplicadas en §4 |
| V | PARCIAL | no existe proyecto de pruebas de integración |
| VI | PASS | Costo estimado declarado en §6 |
| VII | PASS | paginación obligatoria aplicada |

> Sin excepciones registradas (Apéndice E de la constitución).

## 9. Riesgos de implementación

- Consultas geoespaciales sin índice degradan la latencia
- Un radio grande multiplica resultados y costo de lectura

## 10. Orden de implementación

1. `T007-01` Actualizar y validar el contrato OpenAPI del spec
2. `T007-02` Implementar: GetNearbyOffersQuery, validador y proyección a DTO
3. `T007-03` Implementar: Consulta geoespacial con índice 2dsphere y filtro de estado
4. `T007-04` Implementar: Endpoint GET /api/v1/offers/nearby
5. `T007-05` Definir el DTO de oferta cercana con distancia
6. `T007-06` Verificar el uso del índice 2dsphere con explain
7. `T007-07` Validar radio y pageSize en el borde
8. `T007-08` Implementar las pruebas de integración del flujo completo
9. `T007-09` Actualizar `specs/README.md`, `requirements.md` y el checklist del spec
10. `T007-10` Registrar evidencias del Constitution Check
11. `T007-11` Documentar la equivalencia SQL Server → MongoDB (`spec.md` §8)
12. `T007-12` Registrar el mapeo SE-008/011/012/013/016 → FR (`spec.md` §9)
13. `T007-13` Validar el radio 1–5 km registrado y 2 km invitado (`FR-007-08`, `FR-007-09`)
