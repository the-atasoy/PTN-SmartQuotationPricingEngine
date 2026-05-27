# Tasks — Products

Agile task breakdown for product-related features. Depends on Epic 3 (Authentication) being complete.

---

## TASK-013 — Get All Products Endpoint
**Layer:** Backend — Application + API  
**Description:**  
Implement `GetAllProductsQuery` and `GetAllProductsQueryHandler`. Returns `ApiResponse<List<ProductDto>>` with fields: id, name, category, base_price, last_request_price, last_request_date. Create `ProductsController` with `GET /api/products` protected by `[Authorize(Roles = "User,Admin")]`.

**Acceptance Criteria:**
- Returns `ApiResponse` with `isSuccessful: true` and all 6 seeded products in `data`.
- Returns `ApiResponse` with `isSuccessful: false`, `statusCode: 401` when called without a token.
- Each product in `data` includes all fields defined in `ProductDto`.

---

## TASK-014 — Get Price History Endpoint
**Layer:** Backend — Application + API  
**Description:**  
Implement `GetPriceHistoryQuery` and `GetPriceHistoryQueryHandler`. Accepts `productId`, `page` (default 1), `pageSize` (default 10). Returns `ApiResponse<PaginatedResult<PriceHistoryDto>>` with fields: id, quoted_price, created_at, request_id, request_no, customer_name. Ordered by `created_at` descending. Add `GET /api/products/{id}/price-history?page=1&pageSize=10` to `ProductsController` protected by `[Authorize(Roles = "Admin")]`.

**Acceptance Criteria:**
- Returns `ApiResponse<PaginatedResult<PriceHistoryDto>>` with history rows for a valid product ID.
- Pagination metadata (`totalCount`, `totalPages`, `hasNextPage`, `hasPreviousPage`) is correct.
- Returns `ApiResponse` with `isSuccessful: false`, `statusCode: 404` for a non-existent product ID.
- Returns `ApiResponse` with `isSuccessful: false`, `statusCode: 403` when called with a `User` role token.
- Rows are sorted descending by date.

---

## TASK-015 — Product Catalog Page
**Layer:** Frontend  
**Description:**  
Create `/[locale]/products/page.tsx`. Fetch products from `GET /api/products` with auth token. Read `ApiResponse.data` for the product list. Render `ProductGrid` with `ProductCard` components. Each card shows: name, category badge (color-coded per category), base price formatted with currency symbol, **"Sepete Ekle"** button. Create `CategoryFilter` component with buttons for All / HMI / Led Panel / LCD — filters the visible cards client-side. Navbar shows cart item count badge when cart is non-empty. Handle `ApiResponse.isSuccessful === false` by showing a localized error.

**Acceptance Criteria:**
- All 6 products render correctly.
- Category filter shows only matching products.
- Clicking "Sepete Ekle" adds the product to `CartContext` and updates the badge.
- Clicking the same product twice increments quantity rather than adding a duplicate.
