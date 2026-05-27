# Tasks — Authentication

Agile task breakdown for authentication on both backend and frontend. Depends on Epic 2 (Database) being complete.

---

## TASK-009 — JWT Token Service
**Layer:** Backend — Infrastructure  
**Description:**  
Create `ITokenService` interface in `Application/Interfaces/` with methods: `GenerateAccessToken(User user)`, `GenerateRefreshToken()`, `GetPrincipalFromExpiredToken(string token)`. Implement `TokenService` in `Infrastructure/Services/`. Access token lifetime: 15 minutes. Refresh token: cryptographically random string. JWT claims: `sub`, `email`, `role`, `iat`, `exp`. Read secret, issuer, and audience from `appsettings.json` under `Jwt` section.

**Acceptance Criteria:**
- Generated access token is a valid JWT decodable with the configured secret.
- Claims contain correct `role` value from the `User` entity.
- `GetPrincipalFromExpiredToken` returns claims from an expired but otherwise valid token.

---

## TASK-010 — Login & Refresh Endpoints
**Layer:** Backend — Application + API  
**Description:**  
Implement `LoginCommand` with `LoginCommandHandler` and `LoginCommandValidator`. Handler: find user by email, verify BCrypt hash, generate access and refresh tokens, store refresh token hash on the user record with expiry. Create `AuthController` with `POST /api/auth/login` (returns `access_token` in body, sets `refresh_token` as httpOnly Secure cookie) and `POST /api/auth/refresh` (reads cookie, validates, issues new access token). Add `POST /api/auth/logout` that clears the cookie.

**Acceptance Criteria:**
- `POST /api/auth/login` with valid credentials returns `ApiResponse<LoginResponseDto>` with `isSuccessful: true`.
- `POST /api/auth/login` with wrong password returns `ApiResponse` with `isSuccessful: false`, `statusCode: 401`.
- `POST /api/auth/login` with invalid email format returns `ApiResponse` with `isSuccessful: false`, `statusCode: 422`, and validation errors in `errors`.
- `POST /api/auth/refresh` with valid cookie returns new `access_token` in `ApiResponse`.
- `POST /api/auth/logout` clears the cookie.
- Refresh token cookie is `httpOnly` and `SameSite=Strict`.

---

## TASK-011 — Frontend Auth Context & Login Page
**Layer:** Frontend  
**Description:**  
Create `AuthContext.tsx` holding `accessToken`, `role`, `login()`, `logout()`. Token stored only in React state — never written to `localStorage`. Create `LoginForm` component using React Hook Form + Zod validation (schema in `lib/validations/auth.ts`). On successful login, check `ApiResponse.isSuccessful`, decode the JWT to extract `role` and store in context. On failure, display `ApiResponse.message` as a localized error toast. Set a non-httpOnly `auth_meta` cookie with `{ role, exp }` for use by `middleware.ts`. Create `/[locale]/login/page.tsx`.

**Acceptance Criteria:**
- Submitting the login form with valid credentials stores the token in context and redirects to `/products`.
- Submitting with invalid credentials shows a localized error toast.
- Refreshing the page with a valid refresh token cookie restores the session automatically.
- `accessToken` is never present in `localStorage` or `sessionStorage`.

---

## TASK-012 — Frontend Route Guard Middleware
**Layer:** Frontend  
**Description:**  
Create `middleware.ts` at `src/`. Use `next-intl` middleware and compose it with the auth guard. Rules:
- Unauthenticated user accessing any protected route → redirect to `/[locale]/login`.
- Authenticated `User` role accessing any `/admin/*` route → redirect to `/[locale]/products`.
- Authenticated user accessing `/login` → redirect to `/[locale]/products`.
Read role and expiry from the `auth_meta` cookie (not the access token).

**Acceptance Criteria:**
- Directly navigating to `/admin/requests` without auth redirects to `/login`.
- A `User` role token cannot access `/admin/*` routes.
- An `Admin` role token can access both `/products` and `/admin/*`.
