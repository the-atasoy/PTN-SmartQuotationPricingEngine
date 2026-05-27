# Tasks — Quotation Management (Admin Flow)

Agile task breakdown for the admin quotation management flow. Depends on Epic 5 (Request Creation) being complete.

---

## TASK-018 — Get All Requests Endpoint
**Layer:** Backend — Application + API  
**Description:**  
Implement `GetAllRequestsQuery` and `GetAllRequestsQueryHandler`. Accepts `page` (default 1), `pageSize` (default 10). Returns `ApiResponse<PaginatedResult<RequestListItemDto>>` with fields: id, request_no, customer_name, customer_email, total_amount, currency, status, created_at, sent_at. Ordered by `created_at` descending. Add `GET /api/requests?page=1&pageSize=10` to `RequestsController` protected by `[Authorize(Roles = "Admin")]`.

**Acceptance Criteria:**
- Returns `ApiResponse<PaginatedResult<RequestListItemDto>>` with paginated requests.
- Pagination metadata (`totalCount`, `totalPages`, `hasNextPage`, `hasPreviousPage`) is correct.
- Returns `ApiResponse` with `isSuccessful: false`, `statusCode: 403` with a `User` role token.
- Each item includes all fields defined in `RequestListItemDto`.

---

## TASK-019 — Get Request By ID Endpoint
**Layer:** Backend — Application + API  
**Description:**  
Implement `GetRequestByIdQuery` and `GetRequestByIdQueryHandler`. Returns `ApiResponse<RequestDetailDto>` with full `Request` data and nested `RequestItem` list (including product name and category). Add `GET /api/requests/{id}` to `RequestsController` protected by `[Authorize(Roles = "Admin")]`.

**Acceptance Criteria:**
- Returns `ApiResponse` with `isSuccessful: true` and full request detail including items.
- Returns `ApiResponse` with `isSuccessful: false`, `statusCode: 404` for non-existent ID.

---

## TASK-020 — Parse Uploaded Excel Endpoint
**Layer:** Backend — Application + API  
**Description:**  
Implement `ParseUploadedExcelCommand` and handler. Reads uploaded `.xlsx` file using `IExcelService`. For each row, queries `products` table for `last_request_price` and `last_request_date`. Returns `ApiResponse<List<ParsedExcelResultDto>>` with rows: `product_id`, `product_name`, `category`, `quantity`, `last_request_price` (null if none), `last_request_date` (null if none), `has_previous_price` (bool). Create `ExcelController` with `POST /api/excel/parse` protected by `[Authorize(Roles = "Admin")]`.

**Acceptance Criteria:**
- Returns `ApiResponse` with `isSuccessful: true` and enriched rows for a valid Excel upload.
- Rows for products with no price history have `has_previous_price: false`.
- Returns `ApiResponse` with `isSuccessful: false`, `statusCode: 400` if the file is not a valid `.xlsx`.
- Returns `ApiResponse` with `isSuccessful: false`, `statusCode: 422` and validation errors if required columns (`product_id`, `quantity`) are missing.

---

## TASK-021 — Send Quotation Endpoint
**Layer:** Backend — Application + API  
**Description:**  
Implement `SendQuotationCommand` with `SendQuotationCommandHandler` and validator. Input: `request_id`, list of `{ product_id, unit_price, discount }`. Handler steps (single transaction):
1. Validate all `unit_price > 0`.
2. Update `RequestItem` rows with final prices; compute `line_total`.
3. Recompute and update `Request.total_amount`.
4. Set `Request.status = Sent`, `Request.sent_at = now()`.
5. Insert `ProductPriceHistory` row per product.
6. Update `Product.last_request_price` and `Product.last_request_date`.
7. Commit transaction.
8. Send HTML email via `IEmailService` (outside transaction; log failure separately).

Add `PUT /api/requests/{id}/send` to `RequestsController` protected by `[Authorize(Roles = "Admin")]`.

**Acceptance Criteria:**
- All DB changes occur atomically; partial updates do not persist on error.
- Returns `ApiResponse` with `isSuccessful: true` after successful send.
- `products.last_request_price` and `products.last_request_date` reflect the sent prices after the call.
- A new row exists in `product_price_histories` for each product.
- Email appears in Mailpit web UI at `localhost:8025`.
- Returns `ApiResponse` with `isSuccessful: false`, `statusCode: 409` if request status is already `Sent`.
- Returns `ApiResponse` with `isSuccessful: false`, `statusCode: 422` and validation errors if any `unit_price` is 0 or missing.

---

## TASK-022 — Admin Requests List Page
**Layer:** Frontend  
**Description:**  
Create `/[locale]/admin/requests/page.tsx`. Fetch from `GET /api/requests?page=1&pageSize=10`. Read `ApiResponse<PaginatedResult<RequestListItemDto>>`. Render `RequestsTable` with columns: Request No, Customer Name, Customer Email, Date, Total Amount + Currency, Status. Status rendered as color-coded `Badge` (Pending → yellow, Sent → green, Cancelled → red). Each row is clickable and navigates to `/admin/requests/[id]`. Localize status labels. Render pagination controls (previous/next, page numbers) using `PaginatedResult` metadata.

**Acceptance Criteria:**
- All requests render with correct data.
- Pagination controls work correctly (next/previous, page number display).
- Status badges show localized text and correct colors.
- Clicking a row navigates to the detail page.
- Page redirects to login if the user is unauthenticated.

---

## TASK-023 — Admin Request Detail & Quotation Editor Page
**Layer:** Frontend  
**Description:**  
Create `/[locale]/admin/requests/[id]/page.tsx`. Fetch request detail from `GET /api/requests/{id}`. Read `ApiResponse.data`.

**When status is `Pending`:**
- Show `ExcelUploader` (drag-and-drop `.xlsx`, calls `POST /api/excel/parse`).
- On parse result, read `ApiResponse.data` and render `QuotationEditor`: editable table with columns — Product Name, Category, Quantity, Last Price (or "Henüz teklif değeri girilmemiş"), Last Date, Unit Price (editable), Discount % (editable, default 0), Line Total (computed live: `qty * price * (1 - discount/100)`).
- Show grand total below the table, updated live as prices change.
- **Client-side validation:** Use Zod schema (`lib/validations/quotation.ts`) to validate all `unitPrice > 0` and `discount` range before submission. Show inline errors per row.
- **"Teklif İlet"** button: validates form → calls `PUT /api/requests/{id}/send` → checks `ApiResponse.isSuccessful` → on success, shows toast and redirects to `/admin/requests`. On failure, displays `ApiResponse.errors` as toasts.

**When status is `Sent`:**
- Show read-only table of final prices, totals, sent date.

**Acceptance Criteria:**
- Excel upload triggers parse and populates the editor table.
- Line totals and grand total update in real time as unit price or discount changes.
- "Henüz teklif değeri girilmemiş" appears for products with no price history.
- Zod validation prevents submission with `unit_price = 0` and shows inline errors.
- Backend validation errors (`ApiResponse.errors`) are shown as toasts.
- After successful send, Mailpit shows the received email.

---

## TASK-024 — Product Price History Page
**Layer:** Frontend  
**Description:**  
Create `/[locale]/admin/products/[id]/history/page.tsx`. Fetch from `GET /api/products/{id}/price-history?page=1&pageSize=10`. Read `ApiResponse<PaginatedResult<PriceHistoryDto>>`. Render `PriceHistoryTable` with columns: Date, Quoted Price, Request No, Customer Name. Sorted descending by date (already from API). Render pagination controls using `PaginatedResult` metadata. Add a link to this page from product rows in the `QuotationEditor` (product name as clickable link, opens in new tab).

**Acceptance Criteria:**
- All history rows for a product are displayed with pagination.
- Pagination controls work correctly.
- Table is sorted with most recent entry first.
- Page shows an empty state message if no history exists.
