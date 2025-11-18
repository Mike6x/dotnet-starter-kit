# Multitenancy Overview

This module provides tenant lifecycle management and multi-tenant configuration for the starter kit.

## Tenant Lifecycle

- **Create**: `POST api/v1/tenants` - creates a new tenant and runs per-tenant database migrations and seeding.
- **Activation (Preferred)**: `POST api/v1/tenants/{id}/activation` - changes the activation state of a tenant (activate or deactivate) via a single endpoint.
- **Activate / Deactivate (Legacy)**: `POST api/v1/tenants/{id}/activate` and `POST api/v1/tenants/{id}/deactivate` - legacy endpoints for controlling whether a tenant is active. Prefer the unified `/activation` endpoint.
- **Upgrade**: `POST api/v1/tenants/upgrade` - extends or upgrades a tenant subscription.
- **Status**: `GET api/v1/tenants/{id}/status` - returns a `TenantStatusDto` with activation state, validity, and basic metadata.

### Example: Change Activation

```http
POST /api/v1/tenants/{id}/activation HTTP/1.1
Host: localhost:5001
Authorization: Bearer {token}
tenant: {tenant-id}
Content-Type: application/json

{
  "tenantId": "{id}",
  "isActive": true
}
```

Response:

```json
{
  "tenantId": "root",
  "isActive": true,
  "validUpto": "2026-01-01T00:00:00Z",
  "message": "tenant root is now activated"
}
```

## Tenant Resolution Strategies

- **Header**: preferred strategy using the `tenant` header (see `MultitenancyConstants.Identifier`).
- **Claim**: resolved from the `tenant` claim when present on the user principal.
- **Query string**: optional `?tenant=` query parameter, primarily for tooling or diagnostics.

Resolution precedence:

1. Claim-based when user is authenticated and contains the `tenant` claim.
2. Header-based using the `tenant` header.
3. Query string `?tenant=` when explicitly provided.

## Per-Tenant Migrations & Diagnostics

- The platform configures Finbuckle multi-tenancy and per-tenant migrations via `UseHeroMultiTenantDatabases`.
- `MultitenancyOptions.RunTenantMigrationsOnStartup` controls whether tenant migrations/seeding run automatically on startup.

### Diagnostics Endpoint

- **Migrations**: `GET api/v1/tenants/migrations` - returns a collection of `TenantMigrationStatusDto` with:
  - `tenantId`, `name`, `isActive`, `validUpto`
  - `hasPendingMigrations`, `provider`, `lastAppliedMigration`, `pendingMigrations`, `error`

### Health Checks

- `db:multitenancy` - verifies the multitenancy master database is reachable.
- `db:tenants-migrations` - lenient health check that always reports Healthy but includes detailed per-tenant migration information in the result payload.

## Best Practices

- Always include the `tenant` header when calling multi-tenant APIs from clients.
- Use the lifecycle endpoints to manage tenants instead of mutating tenant records manually.
- Use `/health/ready` together with `GET api/v1/tenants/{id}/status` and `GET api/v1/tenants/migrations` to understand platform and per-tenant health.
