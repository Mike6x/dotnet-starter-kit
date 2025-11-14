# FullStackHero .NET 10 Starter Kit

FullStackHero’s .NET 10 Starter Kit is a modular, multi-tenant API platform built on Minimal APIs, Mediator-powered CQRS, and opinionated building blocks. 

## Overview
- Modular vertical slices (`Modules.*`) with dedicated contracts projects and feature folders.
- Building blocks for web, persistence, caching, jobs, mailing, and shared primitives.
- Multiple database providers (PostgreSQL first, MSSQL supported) plus Finbuckle multi-tenancy with per-tenant migrations.
- Cross-cutting features: Serilog logging, RFC7807 errors, rate limiting, HTTP auditing, OpenAPI + Scalar UI, and future Aspire integration.

## Solution Layout
```
src/
  BuildingBlocks/
    Core/            Domain primitives, exceptions, abstractions
    Shared/          Shared contracts, options, constants (e.g., DbProviders)
    Persistence/     EF helpers, DbOptions, interceptors, DbContext bindings
    Web/             Platform host wiring, middleware, versioning, OpenAPI, modules
    Caching/         ICacheService + Redis/memory implementations
    Jobs/            Hangfire integration and filters
    Mailing/         Optional mailing integration
    Storage/         Reserved storage building blocks
  Modules/
    Identity/        Runtime + contracts for authn/authz endpoints
    Multitenancy/    Tenant management runtime, EF store, contracts
    Auditing/        HTTP auditing middleware + contracts
  Playground/
    Playground.Api/          Minimal API host (Program.cs)
    Playground.Blazor*/      Server + WASM playground apps
    Migrations.PostgreSQL/   EF Core migrations (Audit, Identity, Tenancy)
  FSH.Framework.slnx
```

## Build & Tooling Standards
### Shared MSBuild (`Directory.Build.props`)
- Target `net10.0`, latest C# features, nullable + implicit usings enabled.
- Enforcement: `EnforceCodeStyleInBuild=true`, `AnalysisLevel=latest`, SonarAnalyzer wired.
- XML docs generated (1591 suppressed); warnings are not errors yet but should be addressed proactively.

### Central Package Management
- `Directory.Packages.props` carries version declarations; project files only include package names.
- Keep package updates centralized so `dotnet restore FSH.Framework.slnx` remains deterministic.

### Formatting & Analyzers
- `.editorconfig` enforces 4-space indentation, spaces (no tabs), CRLF endings, file-scoped namespaces, explicit types over `var` unless inference improves clarity.
- Run `dotnet format` and fix SonarAnalyzer findings before pushing.

## Runtime Flow
### Host Composition
`Playground/Playground.Api/Program.cs` boots the platform:
1. Gathers module assemblies and registers Mediator with those assemblies.
2. Calls `AddHeroPlatform` to wire logging, CORS, versioning, OpenAPI, health checks, ProblemDetails, optional caching/jobs/mailing.
3. Registers modules through `AddModules(assemblies)`; the loader activates each `IModule`.
4. Configures Finbuckle multi-tenancy and per-tenant database migrations via `UseHeroMultiTenantDatabases`.

### Module Lifecycle
- Modules implement `IModule` with `ConfigureServices` (DI, options, DbContexts) and `MapEndpoints`.
- Endpoints are grouped under `api/v{version:apiVersion}/resource`, tagged, documented, and explicitly authorized with permissions.
- `MapModules()` is invoked once in the pipeline via `UseHeroPlatform(p => p.MapModules = true)`.

### Pipeline Highlights
- Ordering: exception handler → HTTPS → routing → authentication → rate limiting → authorization → OpenAPI/Scalar → endpoints/static files.
- Auditing middleware runs after authentication and captures tenant/user context before authorization executes.

## Building Blocks
- **Core**: Domain abstractions, exceptions, `DomainEventsInterceptor`, helpers for validation and results.
- **Persistence**: `DatabaseOptions`, provider switching, DbContext bindings, interceptors, `IConnectionStringValidator`.
- **Web**: Extension methods for platform setup, OpenAPI, versioning, CORS, rate limiting, and pipeline helpers.
- **Caching**: Distributed cache service abstraction plus Redis/in-memory implementations.
- **Jobs**: Hangfire registration, dashboard, and background worker helpers.
- **Mailing/Storage**: Optional integrations (placeholders for now to be fleshed out per project).

## Feature Modules
- **Identity**: Authentication/authorization endpoints, user/role management, JWT issuing with optional rate-limited token endpoints.
- **Multitenancy**: Tenant CRUD, Finbuckle strategies (header `tenant`, claim, optional query), EF tenant store, per-tenant migrations and seeding.
- **Auditing**: Middleware + services to capture request/response data, mask sensitive payloads, and publish via bounded channels/background sinks.

## Cross-Cutting Capabilities
### CQRS & Validation
- Mediator (source generator) handles commands/queries defined inside `Modules.*.Contracts`; handlers live under module `Features`.
- `ValidationBehavior` automatically runs all FluentValidation validators; `ValidationException` is surfaced as RFC7807 errors.
- Mapster is preferred for DTO ↔ entity mapping.

### Persistence & Database
- `DatabaseOptions` specify provider (`POSTGRESQL` or `MSSQL`), connection string, and migrations assembly.
- `BaseDbContext` applies soft-delete filters, listens for tenant context, and registers interceptors such as domain event dispatching.
- Connection string validators log misconfigurations early; per-tenant contexts are created via `ITenantInfo`.

### Multitenancy
- Finbuckle strategies: header (`tenant`), claim, fallback delegate (query param).
- Stores: EF Core backed + distributed cache for hot lookups.
- Startup: root tenant seed, then per-tenant migrations via `IDbInitializer`.

### API Versioning & Discovery
- `BuildingBlocks/Web/Versioning/Extensions.cs` sets default API version v1 using URL segments.
- Modules build `ApiVersionSet`s and map endpoints under `api/v{version:apiVersion}` to keep OpenAPI accurate.

### OpenAPI & Scalar UI
- Minimal OpenAPI generation plus document transformers injecting metadata from `OpenApiOptions`.
- Scalar UI hosts the API reference with dark theme and pre-configured Bearer auth when JWT is enabled.

### Rate Limiting
- Fixed-window policies partition traffic by tenant → user → client IP.
- Global policy (default 100 req/min) and `auth` policy (default 10 req/min) are configurable via `RateLimitingOptions`.
- Decorate endpoints with `.RequireRateLimiting("auth")` or `.DisableRateLimiting()`; middleware sits between authentication and authorization.

### HTTP Auditing
- Non-blocking middleware captures requests/responses, correlates with W3C Trace Context, and masks sensitive JSON fields (`password`, `token`, `otp`, `pin`, etc.).
- Paths like `/health`, `/metrics`, `/swagger`, `/scalar`, and static framework assets are excluded.
- Body capture is configurable (size caps + opt-out); sinks persist asynchronously through bounded channels.

### CORS
- Single named policy (`FSHCorsPolicy`) driven by `CorsOptions`.
- Supports AllowAll for development; otherwise specify origins, headers, and methods explicitly.

### Error Handling
- `GlobalExceptionHandler` produces RFC7807 payloads:
  - FluentValidation errors → `400` with validation details.
  - `CustomException` respects its provided status and payload.
  - Other exceptions become sanitized problems with correlation info in logs.

### Logging
- Serilog configured via `AddHeroLogging` using the `Serilog` configuration section.
- Enrichers add correlation IDs and log context; noise is reduced for `Microsoft`, `EFCore`, `Hangfire`, `Finbuckle`.
- `appsettings` example uses the compact JSON console formatter; extend with async file sinks as needed.

## Configuration Highlights (`Playground.Api/appsettings.json`)
- `Serilog`: sinks, minimum levels, enrichment.
- `DatabaseOptions`: provider, connection string, migrations assembly.
- `OriginOptions`: base URL for email links/assets.
- `CacheOptions`: Redis connection (empty => in-memory cache).
- `HangfireOptions`: dashboard credentials and route.
- `OpenApiOptions`: title, version, description, contact, license info.
- `CorsOptions`: `AllowAll` toggle plus lists of allowed origins/headers/methods.
- `JwtOptions`: issuer, audience, signing key, token lifetimes.
- `RateLimitingOptions`, `AuditingOptions`, `FshPlatformOptions`: enable/disable cross-cutting features.

### Security Baseline
- Global security headers are applied via `UseHeroSecurityHeaders` (CSP, X-Content-Type-Options, X-Frame-Options, Referrer-Policy).
- JWT auth is configured through `JwtOptions` (issuer, audience, signing key, token lifetimes) and validated at startup.
- For production, run behind HTTPS with HSTS enabled (see Playground Blazor host for an example) and store secrets (like `SigningKey`) outside source control (user secrets, environment variables, or secret managers).

### Health Endpoints
- `/health/live` reports process liveness only (no database calls).
- `/health/ready` runs all registered health checks, including EF Core database checks for Identity, Multitenancy, and Auditing; it returns 503 when any of these checks is unhealthy.

## Development Workflow
### Adding a Module
1. Create `Modules/MyFeature` and `Modules.MyFeature.Contracts`.
2. Implement `IModule` with service registration, DbContexts, and endpoint mapping.
3. Organize features under `Features/v{version}/...`; keep DTOs/contracts in the Contracts project.
4. Register EF migrations (if needed) and hook into `IDbInitializer` for seeding/per-tenant migrations.

### Adding an Endpoint
1. Create a static endpoint class exposing `Map(IEndpointRouteBuilder routes)`.
2. Build or reuse an `ApiVersionSet`, route group, and tags.
3. Apply authorization/permission requirements and rate-limiting annotations.
4. Validate input with FluentValidation; use Mapster for mapping; ensure audit masking if sensitive data is returned.

### API Guidelines
- **Routing**: Place HTTP APIs under `api/v{version:apiVersion}/resource` (for example, `api/v1/identity/users`, `api/v1/tenants`). Use nouns for resources and standard verbs for actions.
- **Versioning**: Define versioned groups with `NewApiVersionSet()` and route groups per module. New versions (`v2`, `v3`, …) should not depend on newer versions.
- **Metadata**: For new endpoints, set `WithName`, `WithSummary`, `WithDescription`, and `Produces` for main responses and errors so OpenAPI/Scalar stay descriptive.
- **Authorization**: Use permission helpers (for example, `RequirePermission(...)`) on endpoints instead of ad‑hoc checks, and tag endpoints consistently (`WithTags("Identity")`, `WithTags("Tenants")`).

### Pre-Commit Checklist
- `dotnet format`
- `dotnet build FSH.Framework.slnx`
- `dotnet test` (once tests exist)
- Review analyzer warnings (SonarAnalyzer) and verify OpenAPI/Scalar load locally
- Update migrations, documentation, and configuration samples when relevant

## Testing Strategy
- Current repo lacks test projects; recommended layout once added:
  - `tests/Domain` for entities/value objects.
  - `tests/Application` for CQRS handlers and validators.
  - `tests/Integration` for API and multi-tenant flows (use Testcontainers + seeded tenants).
  - `tests/Conventions` for architectural rules (naming, dependencies, layering).
- Prefer xUnit with FluentAssertions; name files `<TypeUnderTest>Tests.cs`, tests `Method_State_ExpectedBehavior`.
- Integration tests should boot `Playground.Api`, pass `tenant` headers, and isolate external dependencies via containers or fakes.

## Roadmap
- **Security**: baseline security headers (CSP, HSTS, ReferrerPolicy, X-Content-Type-Options).
- **Observability**: OpenTelemetry traces/metrics/logs, Grafana dashboards, log-trace correlation.
- **Resilience**: Polly v8 pipelines (timeouts, retries, circuit breakers, hedging), response caching + ETags/Brotli.
- **Data & Messaging**: transactional outbox/inbox, message bus abstraction (RabbitMQ/Azure Service Bus/Kafka) with schema versioning.
- **Multitenancy**: schema-per-tenant mode and per-module tenancy strategies.
- **Platform & APIs**: feature flags, typed API clients, Prometheus metrics endpoint, webhook helpers.
- **Developer Experience**: `dotnet new` templates, scaffolding CLI, Dev Containers + docker compose, Testcontainers, Git hooks, Renovate.
- **Operations**: Terraform baseline, Aspire AppHost/ServiceDefaults, infrastructure automation.

### Near-Term Targets
- OpenTelemetry wiring with starter dashboards.
- Resilience policies for outbound HTTP clients.
- Transactional outbox with background dispatcher.
- Output caching + conditional requests for common reads.
- Testcontainers for Identity/Multitenancy integration tests.

## Coming Soon: Aspire Orchestration
An Aspire AppHost/ServiceDefaults experience is planned to orchestrate the API, PostgreSQL, observability stack, and shared services. The existing modular Minimal API composition is designed to slide into Aspire with minimal changes; updates will land here once AppHost scripts are merged.

### Pagination & Sorting
- **Request parameters**: List endpoints that support pagination implement a shared `IPaginationParameters` contract. Parameters are passed via query string:
  - `pageNumber` (optional, 1-based; defaults to 1 when omitted or invalid).
  - `pageSize` (optional; defaults to 20 and is capped at 100).
  - `sort` (optional; multi-column expression like `sort=Name,-ValidUpto`). A `-` prefix means descending; property names are case-insensitive.
- **Response shape**: Paged endpoints return `PagedResponse<T>` with:
  - `items`, `pageNumber`, `pageSize`, `totalCount`, `totalPages`, `hasNext`, and `hasPrevious`.
- **Behavior**: Missing or invalid pagination parameters never produce an error; defaults are applied centrally in the pagination helper. Unknown sort fields are ignored to keep sorting safe.
