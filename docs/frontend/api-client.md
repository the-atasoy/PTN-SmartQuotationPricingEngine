# Frontend — API Client

## Overview

Typed wrapper around `fetch` located at `lib/api/client.ts`. All API communication goes through this client. All responses from the backend follow the `ApiResponse<T>` contract.

---

## ApiResponse Type

The frontend mirrors the backend's `ApiResponse<T>` envelope:

```ts
// lib/types/api-response.ts

interface ApiResponse<T = null> {
  data: T | null;
  isSuccessful: boolean;
  statusCode: number;
  message: string | null;
  errors: string[] | null;
}

interface PaginatedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}
```

All API functions return `ApiResponse<T>` — callers check `isSuccessful` instead of relying on try/catch for expected errors.

---

## Features

- **Base URL** from `NEXT_PUBLIC_API_URL` env var.
- **`Authorization: Bearer`** header injection from `AuthContext`.
- **Automatic token refresh** on 401 (single retry).
- **`Accept-Language`** header set from active `next-intl` locale.
- Parses backend `ApiResponse<T>` envelope and returns it typed.
- On network/unexpected errors, returns a client-side `ApiResponse` with `isSuccessful: false`.

---

## Interface

```ts
async function apiRequest<T>(
  path: string,
  options?: RequestInit & { skipAuth?: boolean }
): Promise<ApiResponse<T>>
```

---

## Usage Pattern

```ts
const response = await getProducts();

if (!response.isSuccessful) {
  // Show error toast with response.message or response.errors
  toast.error(response.message ?? t('errors.generic'));
  return;
}

// Safe to use response.data
const products = response.data;
```

### Handling Validation Errors

```ts
const response = await createRequest(payload);

if (!response.isSuccessful && response.errors) {
  // Display each validation error
  response.errors.forEach(err => toast.error(err));
  return;
}
```

### Handling Paginated Responses

```ts
const response = await getRequests({ page: 1, pageSize: 10 });

if (response.isSuccessful && response.data) {
  const { items, totalPages, hasNextPage } = response.data;
  // Render items and pagination controls
}
```

---

## Module Files

| File | Purpose |
|---|---|
| `client.ts` | Base fetch wrapper with ApiResponse handling + 401 refresh |
| `auth.ts` | Login, refresh, logout API calls |
| `products.ts` | Product listing, price history API calls |
| `requests.ts` | Create request, list requests, get request detail, send quotation |
| `excel.ts` | Parse uploaded Excel API call |

---

## Error Handling Strategy

| Scenario | How it's handled |
|---|---|
| Validation error (422) | `isSuccessful: false`, `errors` array contains field-level messages |
| Not found (404) | `isSuccessful: false`, `message` contains localized message |
| Unauthorized (401) | Client retries with token refresh once, then redirects to login |
| Server error (500) | `isSuccessful: false`, generic localized error message |
| Network error | Client creates a `ApiResponse` with `isSuccessful: false` and a network error message |

All errors are displayed via `sonner` toast notifications. Validation errors from `response.errors` are shown individually.
