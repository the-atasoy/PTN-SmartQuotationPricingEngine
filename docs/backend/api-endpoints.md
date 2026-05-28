# Backend — API Endpoints & Business Logic

## Unified API Response — `ApiResponse<T>`

All API endpoints return a `ApiResponse<T>` envelope. This creates a consistent contract between backend and frontend.

```csharp
public class ApiResponse<T>
{
    public T? Data { get; set; }
    public bool IsSuccessful { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public List<string>? Errors { get; set; }
}
```

### Success response example
```json
{
  "data": { "id": "...", "name": "HMI Panel" },
  "isSuccessful": true,
  "statusCode": 200,
  "message": null,
  "errors": null
}
```

### Validation error response example
```json
{
  "data": null,
  "isSuccessful": false,
  "statusCode": 422,
  "message": "Validation failed",
  "errors": [
    "Email is required",
    "At least one product must be selected"
  ]
}
```

### Generic error response example
```json
{
  "data": null,
  "isSuccessful": false,
  "statusCode": 404,
  "message": "Record not found",
  "errors": null
}
```

---

## Pagination — `PaginatedResult<T>`

List endpoints that may return large datasets support pagination. The paginated response is wrapped inside `ApiResponse<PaginatedResult<T>>`.

```csharp
public class PaginatedResult<T>
{
    public List<T> Items { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
```

### Query Parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| `page` | int | 0 | Page number (0-indexed) |
| `pageSize` | int | 10 | Items per page (max: 50) |

### Paginated response example
```json
{
  "data": {
    "items": [ ... ],
    "page": 0,
    "pageSize": 10,
    "totalCount": 42,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPreviousPage": false
  },
  "isSuccessful": true,
  "statusCode": 200,
  "message": null,
  "errors": null
}
```

---

## Validation — MediatR Pipeline Behavior

Validation is handled automatically via a **MediatR pipeline behavior** (`ValidationBehavior<TRequest, TResponse>`). This acts as middleware for all MediatR commands and queries.

### How it works

1. Every command/query can have a corresponding `FluentValidation` validator (e.g. `LoginCommandValidator`).
2. When a command is sent through MediatR, `ValidationBehavior` runs all registered validators **before** the handler executes.
3. If validation fails, the behavior short-circuits and returns a `ApiResponse` with `IsSuccessful = false`, `StatusCode = 422`, and the list of validation error messages in `Errors`.
4. If validation passes, the request proceeds to the handler.

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Any())
        {
            // Return ApiResponse with validation errors
            // instead of throwing an exception
        }

        return await next();
    }
}
```

### Exception Handler Middleware

Unhandled exceptions (e.g. `NotFoundException`, `UnauthorizedException`) are caught by `ExceptionHandlerMiddleware` in the API layer and converted into `ApiResponse` with the appropriate status code and localized message.

---

## API Endpoints

### Auth

| Method | Route | Auth | Response Type | Description |
|---|---|---|---|---|
| POST | `/api/auth/login` | Public | `ApiResponse<LoginResponseDto>` | Returns `access_token` + sets `refresh_token` httpOnly cookie |
| POST | `/api/auth/refresh` | Cookie | `ApiResponse<LoginResponseDto>` | Issues new access token using refresh token cookie |
| POST | `/api/auth/logout` | Bearer | `ApiResponse` | Clears refresh token cookie |

**Login response body:**
```json
{
  "data": {
    "access_token": "eyJ...",
    "expires_in": 900,
    "role": "Admin"
  },
  "isSuccessful": true,
  "statusCode": 200,
  "message": null,
  "errors": null
}
```

### Products

| Method | Route | Auth | Response Type | Description |
|---|---|---|---|---|
| GET | `/api/products` | `User`, `Admin` | `ApiResponse<List<ProductDto>>` | List all products with base price |
| GET | `/api/products/{id}/price-history?page=0&pageSize=10` | `Admin` | `ApiResponse<PaginatedResult<PriceHistoryDto>>` | Paginated price history for a product |

### Requests

| Method | Route | Auth | Response Type | Description |
|---|---|---|---|---|
| GET | `/api/requests?page=0&pageSize=10` | `Admin` | `ApiResponse<PaginatedResult<RequestListItemDto>>` | Paginated list of quotation requests |
| GET | `/api/requests/{id}` | `Admin` | `ApiResponse<RequestDetailDto>` | Single request with items |
| POST | `/api/requests` | `User` | File download | Create request + return Excel file as download |
| PUT | `/api/requests/{id}/send` | `Admin` | `ApiResponse` | Finalize prices, send email, update product table |

### Excel

| Method | Route | Auth | Response Type | Description |
|---|---|---|---|---|
| POST | `/api/excel/parse` | `Admin` | `ApiResponse<List<ParsedExcelResultDto>>` | Upload Excel, returns rows enriched with last price data |

---

## Key Business Logic

### `POST /api/requests` — Create Request & Export Excel

1. Validate input via `CreateRequestCommandValidator` (customer email required, at least one product line, valid quantities).
2. Create `customer` record (or reuse by email).
3. Create `request` record with status `Pending` and auto-generated `request_no`.
4. Create `request_items` rows with `unit_price = 0` (prices not yet set).
5. Generate Excel file with columns: `request_no`, `product_id`, `product_name`, `category`, `quantity`.
6. Return Excel as `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet` file download.

### `POST /api/excel/parse` — Parse Uploaded Excel

1. Read rows from uploaded `.xlsx`.
2. For each `product_id`, query `products` table for `last_request_price` and `last_request_date`.
3. Return enriched rows wrapped in `ApiResponse`. If no history exists, set `last_request_price: null` and `has_previous_price: false` — frontend will show "Henüz teklif değeri girilmemiş".

### `PUT /api/requests/{id}/send` — Send Quotation (Atomic Transaction)

Entire operation runs inside a single EF Core transaction. On any failure, all changes roll back.

1. Validate via `SendQuotationCommandValidator`: all items must have `unit_price > 0`, valid discount range.
2. Update each `request_item` with final prices and recompute `line_total`.
3. Update `request.total_amount` and `request.status = Sent`, set `request.sent_at`.
4. For each product in the request:
   - Insert row into `product_price_histories`.
   - Update `products.last_request_price` and `products.last_request_date`.
5. Commit transaction.
6. Send email via MailKit (outside transaction — log failure but do not roll back).

---

## Authentication & Authorization

- Access token lifetime: **15 minutes** (JWT Bearer).
- Refresh token lifetime: **7 days** (httpOnly, Secure, SameSite=Strict cookie).
- JWT claims: `sub` (user id), `email`, `role`, `iat`, `exp`.
- `[Authorize(Roles = "Admin")]` on all admin routes.
- `[Authorize(Roles = "User,Admin")]` on product listing and request creation.

Token service interface:
```csharp
public interface ITokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
```
