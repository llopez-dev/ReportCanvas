# ReportCanvas

Visual report builder for professional executive reports — think Canva meets data tables.

## Stack

| Layer | Technology |
|---|---|
| Frontend | Angular 20, Standalone Components, Angular Signals, ECharts, Gridstack.js |
| Backend | ASP.NET Core 10, Clean Architecture, EF Core, PostgreSQL |
| Auth | ASP.NET Core Identity + JWT + Google OAuth |
| Storage | Azure Blob Storage (Azurite locally) |
| Infra | Docker Compose |

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22+](https://nodejs.org)
- [Angular CLI 20+](https://angular.dev/cli): `npm install -g @angular/cli`
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

---

## Local setup

### 1. Start infrastructure (PostgreSQL + Azurite)

```bash
cd infra
docker compose up -d
```

Verify:
- PostgreSQL: `localhost:5432`
- Azurite Blob: `http://localhost:10000`

### 2. Configure environment

```bash
cp .env.example .env
# Edit .env if needed — defaults work for local dev
```

### 3. Run the API

```bash
cd apps/api/src/ReportCanvas.Api

# Apply migrations (first time)
dotnet ef database update

# Run
dotnet run
```

API available at: `https://localhost:5001`  
API Explorer (Scalar UI): `https://localhost:5001/scalar/v1`

### 4. Run the Angular app

```bash
cd apps/web
npm install
ng serve
```

Frontend at: `http://localhost:4200`

---

## EF Core migrations

```bash
# From repo root
dotnet ef migrations add <MigrationName> \
  --project apps/api/src/ReportCanvas.Infrastructure \
  --startup-project apps/api/src/ReportCanvas.Api

dotnet ef database update \
  --project apps/api/src/ReportCanvas.Infrastructure \
  --startup-project apps/api/src/ReportCanvas.Api
```

---

## Project structure

```
report-canvas/
├── apps/
│   ├── api/
│   │   ├── src/
│   │   │   ├── ReportCanvas.Api/           # Controllers, auth, middleware
│   │   │   ├── ReportCanvas.Application/   # Use cases, interfaces, DTOs
│   │   │   ├── ReportCanvas.Domain/        # Entities, enums (no framework deps)
│   │   │   └── ReportCanvas.Infrastructure/# EF Core, storage, parsers
│   │   └── tests/
│   │       ├── ReportCanvas.UnitTests/
│   │       └── ReportCanvas.IntegrationTests/
│   └── web/                                # Angular app
├── infra/
│   └── docker-compose.yml
├── docs/
│   ├── architecture.md
│   ├── data-model.md
│   ├── auth-flow.md
│   └── storage-strategy.md
├── .env.example
└── report-canvas.sln
```

---

## Key endpoints (MVP)

| Method | Route | Auth | Description |
|---|---|---|---|
| GET | `/api/health` | None | Health check |
| POST | `/api/auth/register` | None | Register + create org |
| POST | `/api/auth/login` | None | Login, returns JWT |
| GET | `/api/me` | JWT | Current user info |
| GET | `/api/workspaces/{id}/datasets` | JWT | List datasets |
| POST | `/api/workspaces/{id}/datasets` | JWT | Upload CSV/XLSX |
| GET | `/api/workspaces/{id}/datasets/{id}` | JWT | Get dataset with columns |

---

## NPM packages to install (Angular)

```bash
cd apps/web

# ECharts
npm install echarts ngx-echarts

# Gridstack (drag & drop canvas)
npm install gridstack

# Angular Material + CDK
ng add @angular/material

# HTTP utilities
npm install @auth0/angular-jwt
```

---

## NuGet packages installed

| Project | Package |
|---|---|
| Application | FluentValidation |
| Infrastructure | Npgsql.EntityFrameworkCore.PostgreSQL |
| Infrastructure | Microsoft.AspNetCore.Identity.EntityFrameworkCore |
| Infrastructure | Azure.Storage.Blobs |
| Infrastructure | CsvHelper |
| Infrastructure | ClosedXML |
| Api | Microsoft.AspNetCore.Authentication.JwtBearer |
| Api | Swashbuckle.AspNetCore |
