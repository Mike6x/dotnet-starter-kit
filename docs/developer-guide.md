  ---
  title: Developer Guide – Getting Started
  description: How to set up, run, and start developing with the FullStackHero .NET 10 Starter Kit.
  author: Mukesh
  date: 2025-11-18
  ---

  # Developer Guide – Getting Started

  ## 1. Who This Guide Is For

  This guide is for developers who want to:

  - Clone the FullStackHero .NET 10 Starter Kit.
  - Restore and build the solution.
  - Run the playground API locally.
  - Understand the basic development workflow.

  It assumes familiarity with .NET, C#, and basic command-line usage.

  ## 2. Prerequisites

  Before you start, install:

  - **.NET SDK**: .NET 10 SDK (preview or later that supports `net10.0`).
  - **Node.js + npm** (optional but recommended):
    - Needed if you plan to work on the docs or Blazor playground frontend (where Node-based tooling or docs site exist under `docs/`).
  - **Database**:
    - PostgreSQL (preferred) or SQL Server, depending on your configuration.
  - **Git**:
    - For cloning and working with the repository.

  Make sure `dotnet` and `node` are available on your `PATH`.

  ## 3. Repository Layout (Developer View)

  At a high level:

  - `dotnet-starter-kit/src/`
    - `BuildingBlocks/` – Shared libraries:
      - `Core`, `Shared`, `Persistence`, `Web`, `Caching`, `Jobs`, `Mailing`, `Storage`, `Aspire`.
    - `Modules/`
      - `Identity/`
      - `Multitenancy/`
      - `Auditing/`
    - `Playground/`
      - `Playground.Api/` – Minimal API host and main entry point.
      - `Playground.Blazor*/` – Playground UI projects (when present).
      - `Migrations.PostgreSQL/` – EF migrations for Identity, Multitenancy, Auditing.
    - `FSH.Framework.slnx` – solution file to open in your IDE.

  The main thing you run during development is the **Playground API**.

  ## 4. Initial Setup

  ### 4.1 Clone the repository

  ```bash
  git clone <your-fullstackhero-repo-url>
  cd fullstackhero/dotnet-starter-kit/src

  ### 4.2 Restore .NET dependencies

  From dotnet-starter-kit/src:

  dotnet restore FSH.Framework.slnx

  This uses central package management via Directory.Packages.props, so restore is solution-wide and deterministic.

  ### 4.3 (Optional) Install Node dependencies for docs

  If you plan to work on the documentation site under docs/:

  cd ../../docs
  npm install

  Return to the dotnet-starter-kit/src folder for backend work afterwards.

  ## 5. Configure the Database

  The starter kit is designed for PostgreSQL first, but also supports SQL Server. Configuration details live in the Playground host and its appsettings.

  Typical steps:

  1. Locate the appsettings for Playground.Api:
      - dotnet-starter-kit/src/Playground/Playground.Api/appsettings.json (and environment-specific variants if present).
  2. Configure connection strings:
      - Set the default connection string to point to your local PostgreSQL (or MSSQL) instance.
      - Ensure the database user has permissions to create databases and run migrations.
  3. Confirm the selected provider:
      - The provider is typically configured via options in BuildingBlocks/Shared and the host’s configuration.
      - Make sure the selected provider matches your connection string (for example PostgreSQL).

  You do not need to manage module-specific DbContexts manually; the building blocks and modules handle that based on configuration.

  ## 6. Running Migrations

  The migrations project lives under:

  - dotnet-starter-kit/src/Playground/Migrations.PostgreSQL

  A typical flow from dotnet-starter-kit/src:

  cd Playground/Migrations.PostgreSQL
  dotnet ef database update

  Ensure:

  - The dotnet-ef tool is installed (dotnet tool install --global dotnet-ef if needed).
  - The connection string in the migrations project (or its appsettings) matches your database.

  Depending on your environment, you may also run migrations indirectly via the application startup, where UseHeroMultiTenantDatabases can trigger per-tenant migrations.

  ## 7. Run the Playground API

  From dotnet-starter-kit/src:

  cd Playground/Playground.Api
  dotnet run

  This:

  - Builds the building blocks and modules.
  - Starts an ASP.NET Core Minimal API host.
  - Wires up Hero platform features (logging, CORS, versioning, OpenAPI, rate limiting, auditing, etc.).
  - Registers the Identity, Multitenancy, and Auditing modules.
  - Configures multi-tenant database behavior via UseHeroMultiTenantDatabases.

  By default, the API listens on a local HTTP/HTTPS port. Check the console output for the exact URL (for example https://localhost:<port>).

  ### 7.1 Verify the API is running

  Call the root endpoint:

  curl https://localhost:<port>/

  You should see a JSON response similar to:

  { "message": "hello world!" }

  ### 7.2 Explore OpenAPI / Scalar UI

  With OpenAPI enabled, navigate in a browser to:

  - https://localhost:<port>/swagger or
  - https://localhost:<port>/scalar

  Exact paths may vary based on the current configuration of the Web building block.

  ## 8. Developing with Modules

  ### 8.1 Where to add features

  Follow the module pattern:

  - Add domain and application logic inside the relevant module under src/Modules/*.
  - Use vertical slices:
      - Feature folders (for example Features/v1/Tokens/TokenGeneration in Identity).
      - Each feature has request contracts, handlers, and endpoints.

  ### 8.2 Add a new endpoint (high-level steps)

  1. Choose the module (for example Modules.Identity).
  2. Create or extend a feature folder under that module:
      - Define request/response contracts in Modules.Identity.Contracts.
      - Implement a Mediator command or query handler in Modules.Identity.
  3. Map the endpoint in the module’s MapEndpoints method:
      - Use Minimal APIs, for example MapPost or MapGet.
      - Apply versioning, tags, and authorization policies.
  4. Rebuild and run Playground.Api, then test the new endpoint.

  ## 9. Common Development Commands

  Run from dotnet-starter-kit/src (unless otherwise noted):

  - Build the solution

    dotnet build FSH.Framework.slnx
  - Run tests (if test projects are present under src/Tests)

    cd Tests
    dotnet test
  - Format code

    dotnet format FSH.Framework.slnx

    This respects .editorconfig and the analyzer settings in Directory.Build.props.

  ## 10. Troubleshooting

  ### 10.1 Restore or build issues

  - Ensure you are running a .NET SDK version that supports net10.0.
  - Run dotnet --info and confirm the SDK version.
  - Try a clean build:

    dotnet clean FSH.Framework.slnx
    dotnet restore FSH.Framework.slnx
    dotnet build FSH.Framework.slnx

  ### 10.2 Database connection errors

  - Double-check the connection string in Playground.Api (and migrations project).
  - Verify the database service is running and reachable.
  - Confirm the database user has create/migrate permissions.

  ### 10.3 OpenAPI or UI not available

  - Confirm EnableOpenApi is set to true in Program.cs when calling AddHeroPlatform.
  - Check logs for errors during startup.
  - Ensure you are using the HTTPS URL if OpenAPI is bound to HTTPS.

  ## 11. Next Steps

  Once you are comfortable running the Playground API:

  - Read the Architecture Overview document to understand module boundaries and data flow.
  - Explore each module’s source:
      - Identity – authentication, tokens, and user management.
      - Multitenancy – tenant configuration and status.
      - Auditing – request/response and security audit flows.
  - Start a small feature:
      - Add a simple read-only endpoint to one module.
      - Wire it through a Mediator query and Minimal API endpoint.
      - Document it in your API reference or module guide.