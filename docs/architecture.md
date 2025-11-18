  ---
  title: FullStackHero .NET 10 Starter Kit – Architecture Overview
  description: High-level architecture, modules, and runtime flow for the FullStackHero .NET 10 Starter Kit.
  author: Mukesh Murugan
  date: 2025-11-18
  ---

  # FullStackHero .NET 10 Starter Kit – Architecture Overview

  ## 1. Purpose and Scope

  The FullStackHero .NET 10 Starter Kit is a modular, multi-tenant API platform built on .NET 10 Minimal APIs and Mediator-powered CQRS. It provides reusable building
  blocks and feature modules for identity, multitenancy, and auditing, along with a playground host and migrations.

  This document explains:

  - Core architecture and layering
  - Building blocks and feature modules
  - Runtime startup and HTTP pipeline
  - Data access and multi-tenancy
  - Extension and customization points

  Target readers are intermediate or above .NET developers working on this codebase.

  ## 2. High-Level Architecture

  ### 2.1 Architectural Style

  - **Modular monolith** with clear module boundaries (Identity, Multitenancy, Auditing).
  - **Vertical slice architecture** inside modules (features own their handlers, contracts, and endpoints).
  - **CQRS with Mediator** for commands and queries.
  - **Multi-tenant** infrastructure via Finbuckle.MultiTenant.
  - **ASP.NET Core Minimal APIs** for HTTP endpoints and module mapping.

  ### 2.2 Core Components

  - **BuildingBlocks/** – Shared, reusable libraries:
    - `Core` – Domain primitives, abstractions, exceptions, domain events.
    - `Shared` – Shared contracts, options, constants (for example database provider identifiers).
    - `Persistence` – EF Core helpers, DbContext wiring, interceptors, pagination, specifications.
    - `Web` – Host composition, middleware, versioning, OpenAPI, module loading.
    - `Caching` – Cache abstractions and Redis/in-memory implementations.
    - `Jobs` – Background job support (Hangfire).
    - `Mailing`, `Storage`, `Aspire` – Optional or future integration areas.
  - **Modules/** – Feature boundaries:
    - `Identity` – Authentication, authorization, users, roles, tokens.
    - `Multitenancy` – Tenant resolution, store, migrations, tenant-level configuration.
    - `Auditing` – HTTP and security auditing infrastructure and contracts.
  - **Playground/** – Host applications:
    - `Playground.Api` – Minimal API host that composes building blocks and modules.
    - `Playground.Blazor*` – Playground clients (where present).
    - `Migrations.PostgreSQL` – EF migrations for the main modules.

  ## 3. Solution Layout

  The relevant structure under `dotnet-starter-kit/src` is:

  - `BuildingBlocks/Core` – Domain abstractions, context, exceptions.
  - `BuildingBlocks/Shared` – Shared options and constants.
  - `BuildingBlocks/Persistence` – Database options and EF infrastructure.
  - `BuildingBlocks/Web` – `AddHeroPlatform`, `UseHeroPlatform`, API versioning, OpenAPI, CORS, rate limiting, module wiring.
  - `BuildingBlocks/Caching`, `Jobs`, `Mailing`, `Storage`, `Aspire` – Cross-cutting building blocks.
  - `Modules/Identity` – Runtime module (`Modules.Identity`) and contracts (`Modules.Identity.Contracts`).
  - `Modules/Multitenancy` – Runtime module and contracts.
  - `Modules/Auditing` – Runtime module and contracts.
  - `Playground/Playground.Api` – Composition root and entry point.

  The logical dependency direction is:

  `Playground.Api`
  → `BuildingBlocks.Web` (+ other BuildingBlocks)
  → Feature modules (`Modules.Identity`, `Modules.Multitenancy`, `Modules.Auditing`)
  → `BuildingBlocks.Persistence`, `Core`, `Shared`

  ## 4. Runtime Composition

  ### 4.1 Application Startup

  The main entry point is `Playground/Playground.Api/Program.cs`.

  Key steps:

  1. **Create builder**

     ```csharp
     var builder = WebApplication.CreateBuilder(args);

  2. Register Mediator
      - Uses an AddMediator extension.
      - Registers Mediator with assemblies that contain:
          - Commands and queries (for example GenerateTokenCommand, GetTenantStatusQuery).
          - Their handlers.
          - Module-specific DbContexts (for example AuditDbContext).
  3. Discover module assemblies
      - Builds an array of module assemblies:
          - IdentityModule
          - MultitenancyModule
          - AuditingModule
  4. Configure Hero Platform (building blocks)

     builder.AddHeroPlatform(o =>
     {
         o.EnableCors = true;
         o.EnableOpenApi = true;
         o.EnableCaching = true;
         o.EnableMailing = true;
         o.EnableJobs = true;
     });

     This wires up:
      - Logging (Serilog).
      - CORS.
      - API versioning.
      - OpenAPI and Scalar UI.
      - ProblemDetails error responses.
      - Caching, jobs, mailing when enabled.
  5. Register modules

     builder.AddModules(moduleAssemblies);

     Each module implements a common IModule pattern with:
      - ConfigureServices – Registers module services, DbContexts, options.
      - MapEndpoints – Maps Minimal API endpoints for that module.
  6. Build app and configure pipeline

     var app = builder.Build();
     app.UseHeroMultiTenantDatabases();
     app.UseHeroPlatform(p => { p.MapModules = true; });
      - UseHeroMultiTenantDatabases configures Finbuckle.MultiTenant and per-tenant database behavior.
      - UseHeroPlatform composes the middleware pipeline and maps module endpoints when MapModules is true.
  7. Root endpoint

     A simple root endpoint returns a health message and is allowed anonymously.

  ### 4.2 HTTP Pipeline

  The pipeline is composed in BuildingBlocks/Web and applied via UseHeroPlatform. The order is broadly:

  1. Global exception handling.
  2. HTTPS redirection (when enabled).
  3. Routing.
  4. Authentication.
  5. Rate limiting.
  6. Authorization.
  7. OpenAPI and Scalar UI.
  8. Static files and endpoints.

  The auditing middleware runs after authentication to capture tenant/user context but before authorization and endpoint execution.

  ## 5. Module Architecture

  ### 5.1 Shared Module Contract

  Each module:

  - Has a runtime project (Modules.X) and a contracts project (Modules.X.Contracts).
  - Implements IModule (or an equivalent interface) with:
      - ConfigureServices(IServiceCollection services) – DI registrations, DbContexts, options.
      - MapEndpoints(IEndpointRouteBuilder app) – Minimal APIs grouped under versioned routes.

  Endpoints:

  - Are grouped under versioned paths such as api/v{version:apiVersion}/....
  - Are tagged for OpenAPI.
  - Explicitly apply authorization policies or permissions.

  ### 5.2 Identity Module

  Responsibilities:

  - User and role management.
  - Authentication and authorization.
  - Token generation (JWT), including rate-limited token endpoints when configured.
  - Exposing commands and queries such as token generation via Mediator.

  Typical flows:

  - HTTP endpoint receives a request.
  - Maps to a Mediator command (for example GenerateTokenCommand).
  - Handler uses Identity services and persistence to verify credentials and issue tokens.
  - Response contract defined in Modules.Identity.Contracts.

  ### 5.3 Multitenancy Module

  Responsibilities:

  - Tenant entity and tenant store (often EF-backed).
  - Finbuckle.MultiTenant configuration:
      - Tenant resolution strategies (for example header, claim, optional query).
  - Tenant-aware migrations and seeding.
  - Tenant status and metadata endpoints (for example GetTenantStatusQuery).

  Behaviors:

  - On each request, the multitenancy middleware resolves the current tenant.
  - DbContexts are configured per tenant via connection string resolution.
  - Queries and commands run in a tenant-scoped context.

  ### 5.4 Auditing Module

  Responsibilities:

  - HTTP request/response auditing.
  - Security and activity audit events.
  - Pluggable sinks and masking strategies.

  Key elements:

  - Contracts define:
      - AuditEnvelope and related payloads (activity, entity changes, exceptions).
      - Interfaces like IAuditClient, IAuditSink, IAuditMaskingService.
  - Runtime module provides:
      - Middleware to capture request/response metadata and user/tenant context.
      - Background publishing to sinks, with support for masking sensitive values.
      - EF-based persistence via AuditDbContext when enabled.

  Position in pipeline:

  - Runs after authentication so it can include user and tenant information.
  - Before authorization and the endpoint to capture access attempts as needed.

  ## 6. Building Blocks in Detail

  ### 6.1 Core

  - Domain abstractions and base classes.
  - Exception hierarchy and domain-specific exceptions.
  - Domain events and interceptors (DomainEventsInterceptor).
  - Common result/validation helpers.

  ### 6.2 Persistence

  - DatabaseOptions and multi-provider support:
      - PostgreSQL as the primary provider.
      - MSSQL support.
  - EF Core DbContext helpers and interceptors.
  - Specification, pagination, and query utilities to keep repositories thin.
  - Connection string validation.

  ### 6.3 Web

  - AddHeroPlatform and UseHeroPlatform host extensions.
  - API versioning setup.
  - OpenAPI and Scalar UI configuration.
  - CORS configuration.
  - Rate limiting policies and middleware registration.
  - Module discovery and endpoint mapping helpers.

  ### 6.4 Caching, Jobs, Mailing, Storage, Aspire

  - Caching – ICacheService abstraction plus distributed (Redis) and memory implementations.
  - Jobs – Hangfire integration:
      - Job server and dashboard registration.
      - Filters and helpers for background work.
  - Mailing – Optional mailing building block for SMTP or provider-based integration.
  - Storage – Reserved extension point for storage abstractions.
  - Aspire – Integration hooks for .NET Aspire when used as an orchestrating layer.

  ## 7. Data and Persistence

  ### 7.1 Database Providers

  - Supports multiple database providers, with PostgreSQL emphasized.
  - Migrations for Identity, Auditing, and Multitenancy live in:
      - Playground/Migrations.PostgreSQL.

  ### 7.2 DbContext Model

  - Each module can have its own DbContext (for example AuditDbContext).
  - BuildingBlocks.Persistence provides:
      - Shared abstractions for configuring DbContexts.
      - Interceptors for domain events and auditing.
  - Multitenancy:
      - Per-tenant database or schema selection managed via Finbuckle and configuration.
      - Per-tenant migrations executed via UseHeroMultiTenantDatabases.

  ## 8. Cross-Cutting Concerns

  ### 8.1 Logging and Observability

  - Serilog-based logging configured as part of AddHeroPlatform.
  - Consistent structured logs across modules.
  - HTTP auditing provides an additional layer of observability on requests and responses.

  ### 8.2 Error Handling

  - RFC7807 ProblemDetails responses for errors.
  - Centralized exception handling middleware at the top of the pipeline.
  - Validation failures are mapped to clear, standardized responses.

  ### 8.3 Security

  - Authentication and authorization managed by the Identity module.
  - Permissions modeled in module endpoints.
  - Rate limiting applied per route or policy.
  - Auditing covers security-relevant events via security audit contracts.

  ## 9. Extension Points

  You can extend the platform in several ways:

  - Add a new module (for example Modules.Billing):
      - Create Modules.Billing and Modules.Billing.Contracts.
      - Implement IModule for service registration and endpoint mapping.
      - Add commands, queries, and handlers for core features.