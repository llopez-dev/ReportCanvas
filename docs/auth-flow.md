# Auth Flow — ReportCanvas

## Overview

- **Identity provider:** ASP.NET Core Identity (email/password)
- **Token format:** JWT (Bearer)
- **Planned:** Google OAuth2 (scaffolded, not activated until needed)

---

## Register

```
POST /api/auth/register
{
  "email": "user@example.com",
  "password": "Password123",
  "fullName": "Juan Pérez",
  "organizationName": "Mi Empresa"
}
```

Server-side:
1. Creates `ApplicationUser` via Identity.
2. Creates `Organization` + default `Workspace`.
3. Creates `Membership` with role `Owner`.
4. Returns `AuthResponse` with JWT.

---

## Login

```
POST /api/auth/login
{ "email": "user@example.com", "password": "Password123" }
```

Returns:
```json
{
  "accessToken": "eyJ...",
  "tokenType": "Bearer",
  "expiresIn": 86400,
  "user": { "id": "...", "email": "user@example.com", "fullName": "Juan Pérez" }
}
```

---

## JWT claims

| Claim | Value |
|---|---|
| `sub` | User ID (GUID) |
| `email` | User email |
| `fullName` | Display name |
| `jti` | Unique token ID |

---

## Angular usage

1. Store token in `localStorage` (acceptable for MVP, consider `httpOnly` cookie for production).
2. `AuthInterceptor` attaches `Authorization: Bearer <token>` to every API request.
3. `AuthGuard` redirects to `/auth/login` if token is missing or expired.

---

## Future: Google OAuth

Flow:
1. Angular redirects to `/api/auth/google/challenge`.
2. Server redirects to Google.
3. Callback: `/api/auth/google/callback` → creates user if new → issues JWT.

The `AddAuthentication().AddGoogle()` call is already included in `Program.cs` — just needs credentials from `.env`.
