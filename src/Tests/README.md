# Architecture Tests

This folder contains solution-wide architecture tests for the FullStackHero .NET 10 Starter Kit. The goal is to automatically enforce layering, dependency, and naming rules as the codebase evolves.

## Project

- `Architecture.Tests` targets `net10.0` and lives under `src/Tests/Architecture.Tests`.
- Test dependencies are limited to `xunit`, `Shouldly`, and `AutoFixture`, with versions defined in `src/Directory.Packages.props`.

## What Is Covered

- **Module dependencies**: module runtime projects (`Modules.*`) cannot reference other modules’ runtime projects directly; only their own runtime, contracts, and building blocks are allowed.
- **Playground hosts**: Playground hosts that reference module contracts must also reference the owning runtime module, avoiding tight coupling to contracts alone.
- **Namespace conventions**: selected areas (for example, `BuildingBlocks/Core/Domain`) must declare namespaces that reflect the folder structure.

## Running the Tests

- Run all tests (including architecture tests): `dotnet test src/FSH.Framework.slnx`.
- Architecture tests are lightweight and rely only on project and file structure; they do not require any external services or databases.

## Extending the Rules

- Add new rules as additional test classes inside `Architecture.Tests`, following the existing patterns (using reflection or project file inspection).
- Keep rules fast and deterministic; avoid environment-specific assumptions so the tests remain stable in CI.

