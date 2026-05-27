# Frontend — Pages & Routing

## Route Map

### Public Route
| Route | Access | Description |
|---|---|---|
| `/[locale]/login` | Public | Login form |

### User Routes
| Route | Access | Description |
|---|---|---|
| `/[locale]/products` | `User`, `Admin` | Browse products, add to cart |
| `/[locale]/cart` | `User`, `Admin` | View cart, enter email, get quotation |

### Admin Routes
| Route | Access | Description |
|---|---|---|
| `/[locale]/admin/requests` | `Admin` | All quotation requests (paginated) |
| `/[locale]/admin/requests/[id]` | `Admin` | Upload Excel, edit prices, send quotation |
| `/[locale]/admin/products/[id]/history` | `Admin` | Price history for a product (paginated) |

---

## Page Descriptions

### `/login` — Login

- `LoginForm` component with email and password fields.
- **Validation:** Zod schema validates email format and required fields. Inline errors shown under each field.
- On submit: calls `POST /api/auth/login`. Checks `ApiResponse.isSuccessful`.
- On success: stores token in `AuthContext`, redirects to `/products`.
- On failure: shows `ApiResponse.message` as a localized error toast.

### `/products` — Product Catalog

- Fetches all products via `GET /api/products`. Reads `ApiResponse.data`.
- Displays `ProductGrid` with `ProductCard` components.
- `CategoryFilter` filters by `HMI`, `Led Panel`, `LCD`.
- Each card has name, category badge, base price, and **"Sepete Ekle"** button.
- Navbar shows cart item count badge.

### `/cart` — Cart & Quotation Request

- Lists cart items with quantity controls and remove buttons.
- Shows subtotal per line and grand total.
- `QuoteRequestForm` contains: email input, currency selector (`TRY`/`USD`/`EUR`), **"Teklif Al"** button.
- **Validation:** Zod `quoteRequestSchema` validates email format and currency selection. Inline errors shown. Submit button disabled until valid.
- On submit: `POST /api/requests` → response is a `.xlsx` file → auto-downloaded via `Blob`.
- On backend validation error: `ApiResponse.errors` displayed as individual toasts.
- Cart is cleared after successful download.
- Toast confirms success or shows error.

### `/admin/requests` — Requests List (Paginated)

- `RequestsTable` with columns: Request No, Customer, Date, Total, Currency, Status.
- Status rendered as color-coded `Badge` (`Pending` → yellow, `Sent` → green, `Cancelled` → red).
- Each row links to `/admin/requests/[id]`.
- **Pagination:** Fetches from `GET /api/requests?page=1&pageSize=10`. Uses `PaginatedResult` metadata (`totalPages`, `hasNextPage`, `hasPreviousPage`) to render page controls.

### `/admin/requests/[id]` — Quotation Detail & Send

Split into two sections:

**Section 1 — Excel Upload (visible when status is `Pending`)**
- `ExcelUploader`: drag-and-drop or file picker, accepts `.xlsx` only.
- On upload: `POST /api/excel/parse` → reads `ApiResponse.data` for enriched rows.
- `QuotationEditor`: editable table with columns: Product Name, Category, Quantity, Last Price, Last Date, **Unit Price** (editable input), Discount (editable), Line Total (computed live).
- If `has_previous_price` is false → show "Henüz teklif değeri girilmemiş" in the Last Price cell.
- **Validation:** Zod `sendQuotationSchema` validates all `unitPrice > 0` and `discount` range before submission. Inline errors shown per row.
- **"Teklif İlet"** button: validates form → calls `PUT /api/requests/{id}/send` → checks `ApiResponse.isSuccessful` → on success shows toast, redirects to `/admin/requests`.
- On backend validation error: `ApiResponse.errors` displayed as toasts.

**Section 2 — Read-only View (visible when status is `Sent`)**
- Shows all items with final prices, total, sent date.

### `/admin/products/[id]/history` — Price History (Paginated)

- Table of all historical prices for the product.
- Columns: Date, Quoted Price, Request No, Customer.
- **Pagination:** Fetches from `GET /api/products/{id}/price-history?page=1&pageSize=10`. Renders pagination controls using `PaginatedResult` metadata.
- Page shows an empty state message if no history exists.
