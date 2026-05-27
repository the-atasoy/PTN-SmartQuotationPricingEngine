# Tasks — Email, Localization & Layout

Agile task breakdown for cross-cutting concerns. Email depends on TASK-021 (Send Quotation). Localization and Layout can be implemented in parallel with feature epics.

---

## TASK-025 — Email Service Implementation
**Layer:** Backend — Infrastructure  
**Description:**  
Implement `IEmailService` interface as `EmailService` using MailKit. SMTP config read from `appsettings.json`. Create an HTML email template for quotation delivery containing: customer name, request number, sent date, currency, product table (name, qty, unit price, discount, line total), grand total, company branding (PITON Technology name/address). Subject and greeting are localized using `IStringLocalizer` — send in the customer's language or default `tr`. Register `EmailService` in `Infrastructure/DependencyInjection.cs`.

**Acceptance Criteria:**
- Sending a quotation causes an email to appear in Mailpit at `localhost:8025`.
- Email body is valid HTML with all product rows and totals present.
- Subject line is in the correct language.
- Email failure does not roll back the DB transaction.

---

## TASK-026 — Backend Localization Setup
**Layer:** Backend — API  
**Description:**  
Configure `IStringLocalizer` with **JSON resource files** at `API/Resources/`. Install `My.Extensions.Localization.Json` (or equivalent JSON localization provider). Register localization services in `Program.cs`. Configure `RequestLocalizationMiddleware` to resolve culture from `Accept-Language` header. Supported cultures: `tr` (default), `en`. Create `tr.json` and `en.json` resource files with all required keys. Apply localized messages to: all `FluentValidation` error messages, `NotFoundException`, `UnauthorizedException`, email subject/body.

> **Note:** This project uses JSON files for localization instead of `.resx` files for simpler tooling and easier editing.

**Acceptance Criteria:**
- `GET /api/requests` with `Accept-Language: en` returns English error messages.
- `GET /api/requests` with `Accept-Language: tr` returns Turkish error messages.
- All keys listed in `docs/backend/localization.md` have translations in both JSON files.
- JSON resource files are correctly loaded and resolved.

---

## TASK-027 — Frontend Localization Setup
**Layer:** Frontend  
**Description:**  
Configure `next-intl` with `i18n.ts`. Add `LocaleSwitcher` component to `Navbar`. Locale toggle writes to `NEXT_LOCALE` cookie and reflects immediately in the URL. All UI strings in all pages and components must use `useTranslations()` hook — no hardcoded Turkish or English strings. Add all keys from `messages/tr.json` and `messages/en.json` as defined in the spec.

**Acceptance Criteria:**
- Switching locale updates all visible text immediately without page reload.
- Direct navigation to `/tr/products` and `/en/products` renders the correct language.
- No hardcoded display strings exist outside the message files.
- All status labels, validation messages, and empty state messages are localized.

---

## TASK-028 — Navbar Component
**Layer:** Frontend  
**Description:**  
Create `Navbar.tsx`. Show: app name/logo, navigation links (Products, Admin panel link only for Admin role), `LocaleSwitcher`, cart badge (item count, hidden for Admin role), logout button. On logout: call `POST /api/auth/logout`, clear `AuthContext`, clear `auth_meta` cookie, redirect to `/login`. Navbar must be included in the root `layout.tsx`.

**Acceptance Criteria:**
- Admin role sees the Admin link; User role does not.
- Cart badge shows correct item count.
- Logout clears session and redirects to login.
- `LocaleSwitcher` is visible on all pages.

---

## TASK-029 — Frontend UI Components Library
**Layer:** Frontend  
**Description:**  
Create shared UI components in `components/ui/`:
- `Button.tsx` — primary, secondary, danger variants with loading state
- `Input.tsx` — text input with label, error state, and helper text
- `Badge.tsx` — color-coded status badge (yellow, green, red)
- `Modal.tsx` — reusable modal dialog with overlay
- `Spinner.tsx` — loading spinner animation

These components are used across all pages and should follow a consistent design system.

**Acceptance Criteria:**
- All UI components are reusable and accept appropriate props.
- Components support localized labels and messages.
- Loading states are visually distinct and accessible.
