# Architecture — ReportCanvas

## Overview

ReportCanvas is a **Modular Monolith** following **Clean Architecture** principles.  
It is designed to start simple and scale without rewriting.

---

## Dependency graph

```
ReportCanvas.Api
    ↓ depends on
ReportCanvas.Application   ←───  ReportCanvas.Infrastructure
    ↓ depends on
ReportCanvas.Domain
```

**Rule:** Arrows point inward. Domain has **zero** external dependencies.

---

## Layer responsibilities

### Domain (`ReportCanvas.Domain`)
- Pure C# entities and enums.
- No EF Core, no Azure SDK, no HTTP, no framework.
- Business rules belong here as entity methods or value objects.

### Application (`ReportCanvas.Application`)
- Use cases (services/handlers).
- Interfaces for infrastructure contracts: `IFileStorageService`, `IDatasetParser`, `ICurrentUserService`.
- DTOs and request/response records.
- Validators (FluentValidation).
- **No** concrete infrastructure dependencies.

### Infrastructure (`ReportCanvas.Infrastructure`)
- EF Core + Npgsql: `ReportCanvasDbContext`.
- `AzureBlobStorageService` — implements `IFileStorageService`.
- `CsvDatasetParser`, `ExcelDatasetParser` — implement `IDatasetParser`.
- All NuGet packages with external I/O live here.

### Api (`ReportCanvas.Api`)
- ASP.NET Core controllers.
- JWT + Google OAuth setup.
- Swagger.
- CORS for Angular dev server.
- `CurrentUserService` (reads from `HttpContext`).
- `Program.cs` wires everything together via `AddApplication()` + `AddInfrastructure()`.

---

## Storage strategy

```
Local:       Azurite (Docker container, localhost:10000)
Production:  Azure Blob Storage

Containers:
  datasets/  → datasets/{workspaceId}/{datasetId}/original.csv
  assets/    → assets/{workspaceId}/{brandKitId}/logo.png
  exports/   → exports/{workspaceId}/{reportId}/{exportId}.pdf
```

---

## Auth flow

1. User registers → Organization + Workspace created automatically.
2. POST `/api/auth/login` returns a **JWT** (24h by default).
3. Angular stores JWT in memory / `localStorage` and sends it as `Authorization: Bearer <token>`.
4. `[Authorize]` controllers validate via `JwtBearer` middleware.
5. `ICurrentUserService` reads `sub` + `email` claims from the token.

---

## Multi-tenancy model

```
User (IdentityUser)
  └── Membership → Organization (tenant root)
                       └── Workspace (project scoping)
                               ├── Datasets
                               ├── Reports → Pages → Widgets
                               ├── BrandKits
                               └── FileAssets
```

All data is scoped to a `WorkspaceId`. Routes are `/api/workspaces/{workspaceId}/...`.

---

## Widget model

Everything on the canvas is a `Widget`. No special-casing in the editor.

```json
{
  "type": "BarChart",
  "x": 100, "y": 50, "width": 400, "height": 300,
  "configJson": { "title": "Revenue by Month", "showLegend": true },
  "styleJson": { "backgroundColor": "#ffffff", "borderRadius": 8 },
  "dataBindingJson": {
    "datasetId": "...",
    "groupBy": "month",
    "metric": "revenue",
    "aggregation": "sum",
    "sort": "asc"
  }
}
```

---

## Future scaling path

| Now | Later |
|---|---|
| Monolith | Extract bounded contexts as needed |
| Simple services | Add MediatR/CQRS when complexity warrants |
| Single-tenant per org | Row-level security in PostgreSQL |
| Azurite local | Azure Blob Storage (zero code change via env var) |
| Manual migrations | CI/CD pipeline with auto-migration on deploy |
