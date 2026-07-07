# Data Model — ReportCanvas

## Entity relationship overview

```
ApplicationUser (ASP.NET Identity)
│
└── Membership ──┐
                 ▼
           Organization
                 │
                 └── Workspace
                       ├── Dataset
                       │     └── DatasetColumn
                       ├── Report
                       │     ├── ReportPage
                       │     │     └── Widget
                       │     └── ExportJob
                       ├── BrandKit
                       └── FileAsset
```

---

## Entities

### Organization
Tenant root. One user can belong to multiple organizations via Membership.

| Column | Type | Notes |
|---|---|---|
| Id | uuid | PK |
| Name | text | Display name |
| Slug | text | URL-safe unique name |
| LogoUrl | text? | Public URL |
| PrimaryColor | text? | Hex color |

### Workspace
Project-level scope within an organization.

| Column | Type | Notes |
|---|---|---|
| Id | uuid | PK |
| Name | text | |
| Slug | text | |
| OrganizationId | uuid | FK |

### Membership
User ↔ Organization join table with role.

| Column | Type | Notes |
|---|---|---|
| UserId | uuid | FK to AspNetUsers |
| OrganizationId | uuid | FK |
| Role | enum | Owner, Admin, Member, Viewer |
| IsActive | bool | |

### Dataset
Metadata about an uploaded file. The **raw file lives in Blob Storage**, not the DB.

| Column | Type | Notes |
|---|---|---|
| Id | uuid | PK |
| Name | text | User-defined |
| OriginalFileName | text | |
| FileType | text | csv \| xlsx |
| StoragePath | text | datasets/{wId}/{id}/original.csv |
| FileSizeBytes | bigint | |
| RowCount | int | |
| ColumnCount | int | |
| WorkspaceId | uuid | FK |

### DatasetColumn
Parsed column metadata.

| Column | Type | Notes |
|---|---|---|
| Id | uuid | PK |
| Name | text | Header name |
| InferredType | text | string \| number \| date \| boolean |
| ColumnIndex | int | 0-based position |
| DatasetId | uuid | FK |

### Report

| Column | Type | Notes |
|---|---|---|
| Id | uuid | PK |
| Name | text | |
| Description | text? | |
| WorkspaceId | uuid | FK |
| BrandKitId | uuid? | FK (optional) |

### ReportPage

| Column | Type | Notes |
|---|---|---|
| Id | uuid | PK |
| Name | text | |
| PageNumber | int | 1-based |
| Width | int | Canvas width px |
| Height | int | Canvas height px |
| BackgroundColor | text | Hex |
| ReportId | uuid | FK |

### Widget
One row per widget on a page. All config is in JSON columns.

| Column | Type | Notes |
|---|---|---|
| Id | uuid | PK |
| Type | enum | KpiCard, BarChart, TextBlock, etc. |
| X, Y | int | Position on canvas |
| Width, Height | int | Dimensions |
| ZIndex | int | Stacking order |
| ConfigJson | jsonb | Widget-specific config |
| StyleJson | jsonb | Visual styling |
| DataBindingJson | jsonb? | Null for editorial widgets |
| ReportPageId | uuid | FK |

### BrandKit

| Column | Type | Notes |
|---|---|---|
| Id | uuid | PK |
| Name | text | |
| IsDefault | bool | |
| PrimaryColor | text | Hex |
| SecondaryColor | text | Hex |
| AccentColor | text | Hex |
| FontFamily | text | |
| LogoStoragePath | text? | assets/{wId}/{id}/logo.png |
| WorkspaceId | uuid | FK |

### FileAsset
Generic binary assets (logos, images for widgets).

| Column | Type | Notes |
|---|---|---|
| StoragePath | text | assets/{wId}/... |
| ContentType | text | MIME type |
| FileSizeBytes | bigint | |
| WorkspaceId | uuid | FK |

### ExportJob
Tracks async PDF generation (prepared for future implementation).

| Column | Type | Notes |
|---|---|---|
| Status | enum | Pending, Processing, Completed, Failed |
| Format | text | pdf |
| OutputStoragePath | text? | exports/{wId}/{rId}/{id}.pdf |
| ErrorMessage | text? | |
| StartedAt | timestamptz? | |
| CompletedAt | timestamptz? | |
| ReportId | uuid | FK |
