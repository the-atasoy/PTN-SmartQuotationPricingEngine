# Frontend — Architecture & Project Structure

## Overview

Next.js 15 application using the **App Router**. Styled with **Tailwind CSS**. Communicates with the .NET backend via REST. Supports two distinct user roles — **User** (customer-facing product catalog) and **Admin** (quotation management panel). Fully localized in Turkish and English.

---

## Tech Stack

| Concern | Library / Tool |
|---|---|
| Framework | Next.js 15 (App Router) |
| Styling | Tailwind CSS v4 |
| Language | TypeScript |
| Auth | Custom JWT context + `middleware.ts` edge route guard |
| Localization | `next-intl` |
| HTTP Client | `fetch` (native) with a typed API client wrapper |
| Excel Download | Browser native (`Blob` + anchor click) |
| Form Handling | React Hook Form + Zod |
| Validation | Zod schemas (shared between forms and API) |
| State | React Context (cart, auth) — no external state library needed |
| Icons | Lucide React |
| Notifications | Sonner (toast) |
| Containerization | Docker + Docker Compose |
| CI/CD | GitHub Actions |

---

## Project Structure

```
frontend/
├── public/
├── messages/
│   ├── tr.json
│   └── en.json
├── src/
│   ├── app/
│   │   ├── [locale]/
│   │   │   ├── layout.tsx               # Root layout with locale + auth provider
│   │   │   ├── page.tsx                 # Redirect → /products
│   │   │   ├── login/
│   │   │   │   └── page.tsx
│   │   │   ├── products/
│   │   │   │   └── page.tsx             # Product catalog + cart
│   │   │   ├── cart/
│   │   │   │   └── page.tsx             # Cart review + "Teklif Al"
│   │   │   └── admin/
│   │   │       ├── layout.tsx           # Admin layout (Admin role guard)
│   │   │       ├── page.tsx             # Redirect → /admin/requests
│   │   │       ├── requests/
│   │   │       │   ├── page.tsx         # All requests list
│   │   │       │   └── [id]/
│   │   │       │       └── page.tsx     # Request detail + send quotation
│   │   │       └── products/
│   │   │           └── [id]/
│   │   │               └── history/
│   │   │                   └── page.tsx # Product price history
│   │   └── api/
│   │       └── auth/
│   │           └── refresh/
│   │               └── route.ts         # Proxy refresh call (keeps cookie server-side)
│   ├── components/
│   │   ├── layout/
│   │   │   ├── Navbar.tsx
│   │   │   └── LocaleSwitcher.tsx
│   │   ├── auth/
│   │   │   └── LoginForm.tsx
│   │   ├── products/
│   │   │   ├── ProductCard.tsx
│   │   │   ├── ProductGrid.tsx
│   │   │   └── CategoryFilter.tsx
│   │   ├── cart/
│   │   │   ├── CartItem.tsx
│   │   │   ├── CartSummary.tsx
│   │   │   └── QuoteRequestForm.tsx     # Email input + "Teklif Al" button
│   │   ├── admin/
│   │   │   ├── RequestsTable.tsx
│   │   │   ├── ExcelUploader.tsx
│   │   │   ├── QuotationEditor.tsx      # Editable price rows
│   │   │   └── PriceHistoryTable.tsx
│   │   └── ui/
│   │       ├── Button.tsx
│   │       ├── Input.tsx
│   │       ├── Badge.tsx                # Request status badge
│   │       ├── Modal.tsx
│   │       └── Spinner.tsx
│   ├── context/
│   │   ├── AuthContext.tsx
│   │   └── CartContext.tsx
│   ├── hooks/
│   │   ├── useAuth.ts
│   │   └── useCart.ts
│   ├── lib/
│   │   ├── api/
│   │   │   ├── client.ts                # Base fetch wrapper with ApiResponse handling
│   │   │   ├── auth.ts
│   │   │   ├── products.ts
│   │   │   ├── requests.ts
│   │   │   └── excel.ts
│   │   ├── types/
│   │   │   ├── api-response.ts          # ApiResponse<T> + PaginatedResult<T> types
│   │   │   ├── auth.ts
│   │   │   ├── product.ts
│   │   │   ├── request.ts
│   │   │   └── excel.ts
│   │   ├── validations/
│   │   │   ├── auth.ts                  # Login form Zod schema
│   │   │   ├── cart.ts                  # Cart / quote request form Zod schema
│   │   │   └── quotation.ts             # Quotation editor Zod schema (unit_price > 0)
│   │   └── utils.ts
│   ├── middleware.ts                    # Edge route guard
│   └── i18n.ts                         # next-intl config
├── next.config.ts
├── tailwind.config.ts
└── Dockerfile
```

---

## Environment Variables

```env
# .env.local
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXT_PUBLIC_DEFAULT_LOCALE=tr
```
