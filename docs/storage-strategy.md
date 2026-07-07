# Storage Strategy — ReportCanvas

## Principle

**Raw files are never stored in PostgreSQL.**  
Only metadata (filename, size, column types, row count) is stored in the DB.  
All binaries live in Blob Storage.

---

## Containers

| Container | Contents | Example path |
|---|---|---|
| `datasets` | Uploaded CSV/XLSX originals | `datasets/{workspaceId}/{datasetId}/original.csv` |
| `assets` | Logos, images for widgets/brand kits | `assets/{workspaceId}/{brandKitId}/logo.png` |
| `exports` | Generated PDF exports | `exports/{workspaceId}/{reportId}/{exportId}.pdf` |

---

## Environments

| Environment | Provider | Config key |
|---|---|---|
| Local | Azurite (Docker) | `Storage:ConnectionString` → Azurite connection string |
| Production | Azure Blob Storage | `Storage:ConnectionString` → Azure connection string |

**Zero code change** when switching environments — only the connection string changes.

---

## Upload flow (datasets)

```
POST /api/workspaces/{id}/datasets
  multipart/form-data (file, name?)
       │
       ├─ 1. Validate extension (.csv / .xlsx) and size (≤ 50 MB)
       │
       ├─ 2. Upload original to Blob Storage
       │       path: datasets/{workspaceId}/{datasetId}/original.{ext}
       │
       ├─ 3. Parse headers + first 20 rows (type inference)
       │
       ├─ 4. Save Dataset + DatasetColumns to PostgreSQL
       │
       └─ 5. Return DatasetPreviewResponse (columns + preview rows)
```

---

## URL generation

- Use **SAS URLs** (short-lived, signed) in production Azure Blob.
- Use **direct blob URL** in Azurite (no SAS support in dev mode).
- `AzureBlobStorageService.GetDownloadUrlAsync()` handles both cases automatically.

---

## Security

- All containers are **private** (no public access).
- Clients never get direct credentials — they get short-lived SAS URLs from the API.
- File extensions are validated server-side before upload.
- File size capped at 50 MB per upload.
