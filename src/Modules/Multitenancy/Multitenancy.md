# Multitenancy Overview

This module provides tenant lifecycle management and multi-tenant configuration for the starter kit.

## Tenant Lifecycle

- **Create**: `POST api/v1/tenants` – creates a new tenant and runs per-tenant database migrations and seeding.
- **Activate / Deactivate**: `POST api/v1/tenants/{id}/activate` and `POST api/v1/tenants/{id}/deactivate` – control whether a tenant is active.
- **Upgrade**: `POST api/v1/tenants/upgrade` – extends or upgrades a tenant subscription.
- **Status**: `GET api/v1/tenants/{id}/status` – returns a `TenantStatusDto` with activation state, validity, and basic metadata.

## Tenant Resolution Strategies

- **Header**: preferred strategy using the `tenant` header (see `MultitenancyConstants.Identifier`).
- **Claim**: resolved from the `tenant` claim when present on the user principal.
- **Query string**: optional `?tenant=` query parameter, primarily for tooling or diagnostics.

## Best Practices

- Always include the `tenant` header when calling multi-tenant APIs from clients.
- Use the lifecycle endpoints to manage tenants instead of mutating tenant records manually.
- Use `/health/ready` together with `GET api/v1/tenants/{id}/status` to understand platform and per-tenant health.

