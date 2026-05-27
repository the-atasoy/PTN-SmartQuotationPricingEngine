# Frontend — Authentication

## Auth Flow

1. User submits login form → `POST /api/auth/login`.
2. Backend returns `access_token` in body + sets `refresh_token` as httpOnly cookie.
3. `access_token` stored in memory (React context — never `localStorage`).
4. All API requests attach `Authorization: Bearer <access_token>` header.
5. On 401 response, `client.ts` automatically calls `POST /api/auth/refresh`, gets new `access_token`, retries original request once.
6. On logout or refresh failure, context is cleared and user is redirected to `/login`.

---

## `middleware.ts` (Edge Route Guard)

Runs before every request. Logic:

- If route is `/login` and user has a valid token → redirect to `/products`.
- If route is `/admin/*` and role claim is not `Admin` → redirect to `/products`.
- If route is protected and no token → redirect to `/login`.

Token presence is checked via a non-httpOnly session cookie (`auth_meta`) that stores only `{ role, exp }` — never the actual token. The real `access_token` stays in memory; this cookie only enables the middleware to make routing decisions at the edge without exposing secrets.

---

## `AuthContext.tsx`

```ts
interface AuthState {
  accessToken: string | null;
  role: 'User' | 'Admin' | null;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
}
```

---

## Security Notes

- `accessToken` is **never** persisted in `localStorage` or `sessionStorage`.
- Only `auth_meta` cookie (non-sensitive metadata) is used for edge middleware decisions.
- Refresh token is httpOnly, Secure, SameSite=Strict — never accessible to JavaScript.
