# .NET 10 Starter Kit Architecture - FullStackHero

This document defines the architecture, conventions, and developer guidelines for the FullStackHero .NET 10 Starter Kit. It reflects the current solution as implemented in this repository and serves as the single source of truth for API developers.

Note: Aspire orchestration is planned and will be added soon. The guidance below documents the current single-host composition so you can build productively today and transition smoothly to Aspire later.

## Project Overview

The solution implements a modular, multi-tenant, clean architecture using:
- Minimal APIs with a lightweight module loader
- CQRS via Mediator (source generator) + FluentValidation
- EF Core for data access (PostgreSQL primary, MSSQL optional)
- Finbuckle.MultiTenant for tenant resolution and isolation
- Serilog logging, RFC7807 problem details, and OpenAPI + Scalar UI

### Solution Structure (current)

```
src/
  BuildingBlocks/
    Core/                          # Core primitives (domain, exceptions, etc.)
    Shared/                        # Shared contracts/options/constants (e.g., DbProviders)
    Persistence/                   # EF Core helpers, DbOptions, interceptors
    Web/                           # Web platform, middleware, versioning, OpenAPI, modules
    Caching/                       # ICacheService + Redis/memory
    Jobs/                          # Hangfire integration and filters
    Mailing/                       # Mailing integration (optional)
    Storage/                       # Storage building blocks (reserved)

  Modules/
    Identity/
      Modules.Identity/            # Identity runtime (endpoints, services, data)
      Modules.Identity.Contracts/  # Identity contracts (DTOs, commands, queries)
    Multitenancy/
      Modules.Multitenancy/        # Multitenancy runtime, endpoints, EF store, setup
      Modules.Multitenancy.Contracts/
      Modules.Multitenancy.Web/    # Web glue/types (if any)
    Auditing/
      Modules.Auditing/            # Audit runtime, middleware, publisher
      Modules.Auditing.Contracts/  # Audit contracts

  Playground/
    Playground.Api/                # API host (Program.cs)
    Playground.Blazor/             # Blazor Server app (optional playground)
    Playground.Blazor.Client/      # Blazor WASM client (optional playground)
    Migrations.PostgreSQL/         # EF Core migrations for PostgreSQL

  FSH.Framework.slnx               # Solution
```

## Build & Configuration Standards

### Directory.Build.props (common MSBuild)
- Target Framework: net10.0
- Language version: latest, Nullable enabled, Implicit usings enabled
- Enforce code style in build: enabled
- SonarAnalyzer.CSharp for static analysis
- Generate XML docs (1591 suppressed)
- TreatWarningsAsErrors: off (by choice for now)

### Central Package Management
- `Directory.Packages.props` manages package versions centrally
- No package lock files are used at the moment

### Formatting & Analyzers
- Use `dotnet format` before commits
- SonarAnalyzer insights should be reviewed and addressed where applicable

## Technology Stack

### Core
- .NET 10.0, Minimal APIs
- Mediator (source generator) for CQRS messaging and handlers
- FluentValidation for request validation via pipeline behavior
- Serilog for structured logging
- ProblemDetails and a global exception handler for errors

### Data
- EF Core 10.0 (primary)
- PostgreSQL via Npgsql (primary); MSSQL supported via provider switch
- Connection string validation for both providers

### Multitenancy
- Finbuckle.MultiTenant with multiple strategies: header ("tenant"), claim, and optional query param
- EF Core store with distributed cache store for tenant info
- Per-tenant database migration and seeding on startup

### API Docs & UI
- Minimal OpenAPI (Microsoft.AspNetCore.OpenApi)
- Scalar.AspNetCore for API reference UI
- Automatic Bearer token security scheme injection when JWT is enabled

## Runtime Composition

Entry point: `Playground/Playground.Api/Program.cs`
- Adds Mediator with feature/contract assemblies
- Adds FullStackHero platform via `AddFshPlatform`:
  - Logging, CORS, Versioning, OpenAPI, Health checks, ProblemDetails
  - Optional: Caching, Jobs, Mailing
- Loads modules via `AddModules(assemblies)` (reflection-based) and maps endpoints via `UseFshPlatform(p => p.MapModules = true)`
- Configures Finbuckle multi-tenant middleware and per-tenant DB migrations via `ConfigureMultiTenantDatabases()`

## Platform (BuildingBlocks/Web)

`FSH.Framework.Web.Extensions` wires cross-cutting concerns:
- Services: logging, HttpContextAccessor, DB options, CORS (from config), versioning, OpenAPI, health checks
- Middleware: exception handler, HTTPS redirection, routing, OpenAPI, static files, authN/Z
- Endpoint mapping: `MapModules()` once modules are discovered/registered

Configuration flags via `FshPlatformOptions` and `FshPipelineOptions` enable/disable CORS, OpenAPI, caching, jobs, mailing, etc.

## Module System (Minimal APIs)

Modules implement `IModule` with two responsibilities:
- `ConfigureServices(IHostApplicationBuilder builder)`: register services, DbContexts, options
- `MapEndpoints(IEndpointRouteBuilder endpoints)`: map versioned Minimal API endpoints

`ModuleLoader` scans assemblies, instantiates modules, calls `ConfigureServices`, and later maps their endpoints through `MapModules()`.

Endpoint groups should:
- Use `api/v{version:apiVersion}/...` prefix
- Set tags, summary, and description
- Apply authorization and permissions explicitly

Example: `Modules.Multitenancy` maps its group and endpoints in `MultitenancyModule.cs` and uses `Asp.Versioning` to build a version set.

## API Versioning

`BuildingBlocks/Web/Versioning/Extensions.cs` configures:
- Default API version v1
- URL segment version reader
- API Explorer support with version substitution

Usage pattern:
- Build a version set in the module
- Map endpoints under `api/v{version:apiVersion}/<resource>`

## OpenAPI & Scalar UI

`BuildingBlocks/Web/OpenApi/Extensions.cs` enables:
- Minimal OpenAPI document generation with a document transformer for metadata (title, version, contact, license) bound from `OpenApiOptions`
- Scalar UI mounted with a theme, dark mode, and Bearer auth preference

`BearerSecuritySchemeTransformer` automatically injects a Bearer security scheme and requirement for all operations when JWT authentication is configured.

## Rate Limiting

Built-in, configuration-driven rate limiting protects APIs from abuse while staying tenant- and user-aware.

- Global policy: fixed window (default 100 requests/minute), partitioned by Tenant ID, then User ID, falling back to client IP.
- Auth policy: fixed window (default 10 requests/minute) intended for token-generation and other sensitive endpoints.
- Health endpoints are exempt via `.DisableRateLimiting()` and a path guard; static files are served before rate limiting middleware and are unaffected.
- Middleware order: after `UseAuthentication()` and before `UseAuthorization()` to allow user/tenant partitioning.
- Endpoint overrides: use `.RequireRateLimiting("auth")` or `.DisableRateLimiting()` on specific endpoints/groups.

Configuration (`RateLimitingOptions`):

```
"RateLimitingOptions": {
  "Enabled": true,
  "Global": { "PermitLimit": 100, "WindowSeconds": 60, "QueueLimit": 0 },
  "Auth": { "PermitLimit": 10,  "WindowSeconds": 60, "QueueLimit": 0 }
}
```

Registration and pipeline:
- Services: `builder.Services.AddHeroRateLimiting(builder.Configuration)`
- Middleware: `app.UseHeroRateLimiting()` (inserted between `UseAuthentication` and `UseAuthorization`)

Example usage:
- The token endpoint is annotated with `.RequireRateLimiting("auth")`.

## CORS

`BuildingBlocks/Web/Cors/Extensions.cs` reads `CorsOptions` from configuration and exposes a single named policy (`FSHCorsPolicy`).
- AllowAll for development is supported
- Otherwise, explicitly configured origins, headers, and methods are applied

## Error Handling & Problem Details

Global exception handling is provided by `GlobalExceptionHandler` (RFC7807):
- FluentValidation exceptions render a 400 with error details
- `CustomException` propagates its status code and optional errors collection
- Other exceptions return a generic problem with the exception message

The handler integrates with Serilog and includes contextual details.

## Logging

Serilog integration is configured via `AddHeroLogging`:
- Reads configuration from appsettings
- Enriches context (including correlation ID)
- Adjusts noise levels for framework and EF logs

## CQRS with Mediator

The solution uses `Mediator` with source generator:
- Commands and queries are defined in `*.Contracts` projects
- Handlers live in module `Features` directories
- The `ValidationBehavior` runs automatically for all messages with registered validators

Handler example (style used in repository):

```csharp
public sealed class CreateTenantCommandHandler(ITenantService service)
  : ICommandHandler<CreateTenantCommand, CreateTenantCommandResponse>
{
    public async ValueTask<CreateTenantCommandResponse> Handle(CreateTenantCommand command, CancellationToken ct)
    {
        var id = await service.CreateAsync(command.Id, command.Name, command.ConnectionString, command.AdminEmail, command.Issuer, ct);
        return new CreateTenantCommandResponse(id);
    }
}
```

Validation:
- Add `FluentValidation` validators per command/query
- Invalid messages throw `ValidationException` and are rendered as problem details

Mapping:
- Use Mapster where needed for DTO to entity/read model mapping

## Persistence & Database

`DatabaseOptions` (bound from configuration):
- `Provider`: `POSTGRESQL` (default) or `MSSQL`
- `ConnectionString`: required
- `MigrationsAssembly`: assembly containing EF migrations

Provider configuration is handled by `OptionsBuilderExtensions.ConfigureDatabase`, switching between Npgsql and SqlServer providers and applying the migrations assembly.

`BaseDbContext` (multi-tenant DbContext):
- Applies a soft-delete global query filter
- Configures provider based on the current tenant’s connection string when available
- Enables sensitive data logging in development

Domain events:
- `DomainEventsInterceptor` publishes domain events after `SaveChangesAsync`
- To use, register it as an `ISaveChangesInterceptor`; `BindDbContext` will add any registered `ISaveChangesInterceptor` automatically

Connection string validation:
- `IConnectionStringValidator` validates PostgreSQL and MSSQL connection strings and logs any parsing errors

## Multitenancy

Configured in `MultitenancyModule`:
- Strategies: claim, header (`tenant`), and optional delegate (query parameter)
- Stores: EF Core store + distributed cache store
- On startup: root tenant seed if missing; then per-tenant migrations and seeding via `IDbInitializer`

Header names and defaults are in `MultitenancyConstants` (e.g., `Identifier = "tenant"`).

## App Configuration (Playground.Api)

Key sections in `Playground/Playground.Api/appsettings.json`:
- `Serilog`: console sink and minimum levels
- `DatabaseOptions`: provider, connection string, migrations assembly
- `OriginOptions`: origin URL (used by mailers/identity images, etc.)
- `CacheOptions`: Redis connection (empty => memory cache)
- `HangfireOptions`: dashboard username, password, and route
- `OpenApiOptions`: title, version, description, contact, license
- `CorsOptions`: allow-all or explicit origins/headers/methods
- `JwtOptions`: issuer, audience, signing key, token lifetimes

## Testing Strategy

Current repository (src) does not include test projects. Recommended structure once added:
- `Tests/Domain` – entity/value object tests
- `Tests/Application` – command/query handler tests
- `Tests/Integration` – API and multitenant integration scenarios (consider Testcontainers)
- `Tests/Conventions` – architectural/convention rules

Guidelines:
- Validate domain rules and boundaries with unit tests
- Test endpoints via integration tests (supply `tenant` header when required)
- Add tests for validators and authorization rules

## Developer How-To

Add a new module:
- Create `Modules/MyModule` and `Modules.MyModule.Contracts`
- Implement `IModule` with `ConfigureServices` (DI, DbContexts) and `MapEndpoints` (versioned Minimal APIs)
- Organize features under `Features/v{n}/...` with endpoints, commands/queries, and validators
- Keep DTOs/contracts in the Contracts project; do not leak domain entities over API
- If persistence is needed, add a DbContext and optionally an `IDbInitializer` for migrations/seeding

Add an endpoint to an existing module:
- Create a static endpoint class with `Map(IEndpointRouteBuilder)` returning `RouteHandlerBuilder`
- Use route groups, tags, summary, description, and permissions
- Bind inputs explicitly; validate with FluentValidation

## Pre-Commit Checklist

- `dotnet format` – apply formatting
- `dotnet build` – ensure compilation succeeds
- `dotnet test` – run when test projects exist
- Review analyzer warnings (SonarAnalyzer) and address critical findings
- Verify OpenAPI JSON and Scalar UI load locally

## Future: Aspire Orchestration (Planned)

We will introduce an AppHost + ServiceDefaults to orchestrate resources and local development workflows. Initial targets:
- API project as a hosted service
- PostgreSQL database resource
- Logging/observability surface (e.g., Seq, dashboard)
- Seamless local run, environment parity, and deployment bridges

This document will be updated when Aspire is integrated. The current modular, Minimal API and configuration patterns are designed to transition cleanly to Aspire.
