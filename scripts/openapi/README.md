# OpenAPI Client Generation

Use NSwag (local dotnet tool) to generate typed C# clients + DTOs from the Playground API spec.

## Prereqs
- .NET SDK (repo already uses net10.0)
- Local tool manifest at `.config/dotnet-tools.json` (created) with `nswag.consolecore`

## One-liner
```powershell
./scripts/openapi/generate-api-clients.ps1 -SpecUrl "https://localhost:7030/openapi/v1.json"
```

This restores the local tool, ensures the output directory exists, and runs NSwag with the spec URL you provide.

## Output
- Clients + DTOs: `src/Playground/Playground.Blazor/ApiClient/Generated.cs` (single file; multiple client types grouped by first path segment after the base path, e.g., `/api/v1/identity/*` -> `IdentityClient`).
- Namespace: `FSH.Playground.Blazor.ApiClient`
- Client grouping: `MultipleClientsFromPathSegments`; ensure Minimal API routes keep module-specific first segments.
- Bearer auth: configure `HttpClient` (via DI) with the bearer token; generated clients use injected `HttpClient`.

## Tips
- If the API changes, rerun the script with the updated spec URL (e.g., staging/prod).
- Commit regenerated clients alongside related API changes to keep UI consumers in sync.
