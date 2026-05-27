# Tasks — Request Creation (User Flow)

Agile task breakdown for the user-facing quotation request flow. Depends on Epic 4 (Products) being complete.

---

## TASK-016 — Create Request Endpoint & Excel Export
**Layer:** Backend — Application + API  
**Description:**  
Implement `CreateRequestCommand` with `CreateRequestCommandHandler` and `CreateRequestCommandValidator`. Handler steps:
1. Find or create `Customer` by email.
2. Generate `request_no` in format `RQ-YYYYMMDD-NNN` (sequential per day).
3. Create `Request` with status `Pending`.
4. Create `RequestItem` rows with `unit_price = 0`.
5. Use `IExcelService` to generate `.xlsx` with columns: `request_no`, `product_id`, `product_name`, `category`, `quantity`.
6. Return the file as `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`.

Implement `ExcelService` in Infrastructure using EPPlus. Add `POST /api/requests` to `RequestsController`.

**Acceptance Criteria:**
- Returns HTTP 200 with a valid `.xlsx` file download.
- The Excel file contains one row per product in the request plus a header row.
- A `Request` record with status `Pending` exists in the DB after the call.
- Returns HTTP 422 if the cart is empty or email is invalid.

---

## TASK-017 — Cart Page & Quotation Request
**Layer:** Frontend  
**Description:**  
Create `/[locale]/cart/page.tsx`. Display `CartItem` list with quantity increment/decrement controls and remove button. Show `CartSummary` with subtotal per line and grand total. Render `QuoteRequestForm` at the bottom: email input, currency dropdown (TRY/USD/EUR), **"Teklif Al"** button. Use Zod schema (`lib/validations/cart.ts`) with React Hook Form for client-side validation (email format, currency required). On submit: call `POST /api/requests`, receive `.xlsx` blob, trigger browser download. On backend validation error, display `ApiResponse.errors` as individual toasts. Clear cart on success. Show success toast with localized message.

**Acceptance Criteria:**
- Empty cart shows a localized empty state message with a link back to products.
- Form shows inline Zod validation errors (e.g. invalid email) before submission.
- Successful submission triggers an `.xlsx` file download in the browser.
- Backend validation errors (`ApiResponse.errors`) are shown as toasts.
- Cart is empty after successful submission.
- Loading state disables the button and shows a spinner during the request.
