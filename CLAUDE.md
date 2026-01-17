# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run Commands

```bash
# Restore and build
dotnet restore src/FSH.Framework.slnx
dotnet build src/FSH.Framework.slnx

# Run with Aspire (spins up Postgres + Redis via Docker)
dotnet run --project src/Playground/FSH.Playground.AppHost

# Run API standalone (requires DB/Redis/JWT config in appsettings)
dotnet run --project src/Playground/Playground.Api

# Run all tests
dotnet test src/FSH.Framework.slnx

# Run single test project
dotnet test src/Tests/Architecture.Tests

# Run specific test
dotnet test src/Tests/Architecture.Tests --filter "FullyQualifiedName~TestMethodName"

# Generate C# API client from OpenAPI spec (requires API running)
./scripts/openapi/generate-api-clients.ps1 -SpecUrl "https://localhost:7030/openapi/v1.json"

# Check for OpenAPI drift (CI validation)
./scripts/openapi/check-openapi-drift.ps1 -SpecUrl "<spec-url>"
```

## Architecture

FullStackHero .NET 10 Starter Kit - multi-tenant SaaS framework using vertical slice architecture.

### Repository Structure

- **src/BuildingBlocks/** - Reusable framework components (packaged as NuGets): Core (DDD primitives), Persistence (EF Core + specifications), Caching (Redis), Mailing, Jobs (Hangfire), Storage, Web (host wiring), Eventing
- **src/Modules/** - Feature modules (packaged as NuGets): Identity (JWT auth, users, roles), Multitenancy (Finbuckle), Auditing
- **src/Playground/** - Reference implementation using direct project references for development; includes Aspire AppHost, API, Blazor UI, PostgreSQL migrations
- **src/Tests/** - Architecture tests using NetArchTest.Rules, xUnit, Shouldly
- **scripts/openapi/** - NSwag-based C# client generation from OpenAPI spec; outputs to `Playground.Blazor/ApiClient/Generated.cs`
- **terraform/** - AWS infrastructure as code (modular)
  - `modules/` - Reusable: network, ecs_cluster, ecs_service, rds_postgres, elasticache_redis, alb, s3_bucket
  - `apps/playground/` - Playground deployment stack with `envs/{dev,staging,prod}/{region}/`
  - `bootstrap/` - Initial AWS setup (S3 backend, etc.)

### Module Pattern

Each module implements `IModule` with:
- `ConfigureServices(IHostApplicationBuilder)` - DI registration
- `MapEndpoints(IEndpointRouteBuilder)` - Minimal API endpoint mapping

Feature structure within modules:
```
Features/v1/{Feature}/
├── {Feature}Command.cs (or Query)
├── {Feature}Handler.cs
├── {Feature}Validator.cs (FluentValidation)
└── {Feature}Endpoint.cs (static extension method on IEndpointRouteBuilder)
```

Contracts projects (`Modules.{Name}.Contracts/`) contain public DTOs shareable with clients.

### Endpoint Pattern

Endpoints are static extension methods returning `RouteHandlerBuilder`:
```csharp
public static RouteHandlerBuilder MapXxxEndpoint(this IEndpointRouteBuilder endpoint)
{
    return endpoint.MapPost("/path", async (..., IMediator mediator, CancellationToken ct) =>
    {
        var result = await mediator.Send(command, ct);
        return TypedResults.Ok(result);
    });
}
```

### Platform Wiring

In `Program.cs`:
1. Register Mediator with command/query assemblies
2. Call `builder.AddHeroPlatform(...)` - enables auth, OpenAPI, caching, mailing, jobs, health, OTel
3. Call `builder.AddModules(moduleAssemblies)` to load modules
4. Call `app.UseHeroMultiTenantDatabases()` for tenant DB migrations
5. Call `app.UseHeroPlatform(p => p.MapModules = true)` to wire endpoints

## Configuration

Key settings (appsettings or env vars):
- `DatabaseOptions:Provider` - postgres or mssql
- `DatabaseOptions:ConnectionString` - Primary database
- `CachingOptions:Redis` - Redis connection
- `JwtOptions:SigningKey` - Required in production

## Code Standards

- .NET 10, C# latest, nullable enabled
- SonarAnalyzer.CSharp with code style enforced in build
- API versioning in URL path (`/api/v1/...`)
- Mediator library (not MediatR) for commands/queries
- FluentValidation for request validation
- **Zero warnings policy**: After making any code changes, always verify the build produces no warnings. Run `dotnet build src/FSH.Framework.slnx` and ensure "0 Warning(s)" in output. Fix any warnings before considering work complete.

## Blazor UI Components

The framework provides reusable Blazor components in `BuildingBlocks/Blazor.UI/Components/` with consistent styling.

### FshPageHeader Component

Use `FshPageHeader` for consistent page headers across Playground.Blazor:

```razor
@using FSH.Framework.Blazor.UI.Components.Page

<FshPageHeader Title="Page Title"
               Description="Optional description text">
    <ActionContent>
        <!-- Optional action buttons/controls -->
        <MudButton>Action</MudButton>
    </ActionContent>
</FshPageHeader>
```

**Parameters:**
- `Title` (required): Main page title
- `Description` (optional): Description text below title
- `DescriptionContent` (optional): RenderFragment for complex descriptions
- `ActionContent` (optional): RenderFragment for action buttons on the right
- `TitleTypo` (optional): Typography style (default: Typo.h5)
- `Class` (optional): Additional CSS classes
- `PageTitleSuffix` (optional): Suffix for browser tab title

**Features:**
- Modern card design with MudPaper Elevation="2"
- Subtle gradient background with primary color accent
- Left border accent in primary color
- Dark mode support

### FshUserProfile Component

Modern user profile dropdown for app bars/navbars with avatar, user info, and menu:

```razor
@using FSH.Framework.Blazor.UI.Components.User

<FshUserProfile UserName="@userName"
                UserEmail="@userEmail"
                UserRole="@userRole"
                AvatarUrl="@avatarUrl"
                OnProfileClick="NavigateToProfile"
                OnSettingsClick="NavigateToSettings"
                OnLogoutClick="LogoutAsync" />
```

**Parameters:**
- `UserName` (required): User's display name
- `UserEmail` (optional): User's email address
- `UserRole` (optional): User's role or title
- `AvatarUrl` (optional): URL to user's avatar (shows initials if not provided)
- `ShowUserName` (optional): Show username next to avatar (default: true, hidden on mobile)
- `ShowUserInfo` (optional): Show user info in menu header (default: true)
- `MenuItems` (optional): Custom RenderFragment for menu items (uses default Profile/Settings/Logout if not provided)
- `OnProfileClick` (optional): Callback for Profile menu item
- `OnSettingsClick` (optional): Callback for Settings menu item
- `OnLogoutClick` (optional): Callback for Logout menu item

**Features:**
- Responsive design (hides username on mobile)
- Avatar with initials fallback
- Smooth hover animations and transitions
- Gradient menu header with user info
- Customizable menu items via RenderFragment
- Scoped CSS for isolated styling

### FshStatCard Component

Statistics card for displaying metrics with icon, value, label, and optional badge:

```razor
@using FSH.Framework.Blazor.UI.Components.Cards

<FshStatCard Icon="@Icons.Material.Filled.People"
             IconColor="Color.Primary"
             Value="42"
             Label="Total Users"
             Badge="Active"
             BadgeColor="Color.Success" />
```

**Parameters:**
- `Icon` (required): MudBlazor icon to display
- `IconColor` (optional): Color theme for icon and accent (default: Primary)
- `Value` (required): Main metric value to display
- `Label` (required): Description of the metric
- `Badge` (optional): Small badge text below label
- `BadgeColor` (optional): Color for the badge (default: Primary)

**Features:**
- Hover animation with lift effect (`translateY(-4px)`) and enhanced shadow
- Uses MudCard with Elevation="2" for consistent Material Design styling
- Scoped CSS with `::deep` for proper Blazor CSS isolation
- Consistent structure matching the original stats-card pattern used throughout the app

### FSH Design Tokens

The framework uses CSS custom properties for consistent styling across all components. These are defined in `fsh-theme.css`:

```css
:root {
  /* Border Radius */
  --fsh-radius: 10px;
  --fsh-radius-sm: 8px;
  --fsh-radius-lg: 16px;
  --fsh-radius-xl: 20px;
  --fsh-radius-full: 9999px;

  /* Shadows */
  --fsh-shadow-sm: 0 1px 3px rgba(0, 0, 0, 0.08);
  --fsh-shadow-md: 0 4px 12px rgba(0, 0, 0, 0.08);
  --fsh-shadow-lg: 0 10px 30px rgba(0, 0, 0, 0.1);

  /* Card Styling */
  --fsh-card-bg: #ffffff;
  --fsh-card-border: rgba(0, 0, 0, 0.08);
  --fsh-card-shadow: var(--fsh-shadow-md);

  /* Text Colors */
  --fsh-text-primary: #1a1a2e;
  --fsh-text-secondary: #64748b;

  /* Transitions */
  --fsh-transition: 0.3s cubic-bezier(0.4, 0, 0.2, 1);
}
```

Dark mode overrides are automatically applied when `.mud-theme-dark` is present. Use these tokens in custom components to ensure consistent styling.
