# Frontend — Validation

## Overview

All forms use **Zod** schemas for client-side validation, integrated with **React Hook Form** via `@hookform/resolvers`. Validation errors from the backend (`ApiResponse.errors`) are also displayed to the user.

---

## Validation Schemas (`lib/validations/`)

### `auth.ts` — Login Form

```ts
import { z } from 'zod';

export const loginSchema = z.object({
  email: z
    .string()
    .min(1, 'errors.required')       // Localization key
    .email('errors.invalidEmail'),
  password: z
    .string()
    .min(1, 'errors.required'),
});

export type LoginFormData = z.infer<typeof loginSchema>;
```

### `cart.ts` — Quote Request Form

```ts
import { z } from 'zod';

export const quoteRequestSchema = z.object({
  email: z
    .string()
    .min(1, 'errors.required')
    .email('errors.invalidEmail'),
  currency: z.enum(['TRY', 'USD', 'EUR'], {
    errorMap: () => ({ message: 'errors.required' }),
  }),
});

export type QuoteRequestFormData = z.infer<typeof quoteRequestSchema>;
```

### `quotation.ts` — Quotation Editor (Admin)

```ts
import { z } from 'zod';

export const quotationItemSchema = z.object({
  productId: z.string().uuid(),
  unitPrice: z
    .number()
    .positive('admin.validation.priceRequired'),
  discount: z
    .number()
    .min(0, 'admin.validation.discountMin')
    .max(100, 'admin.validation.discountMax'),
});

export const sendQuotationSchema = z.object({
  items: z
    .array(quotationItemSchema)
    .min(1, 'admin.validation.atLeastOneItem'),
});

export type SendQuotationFormData = z.infer<typeof sendQuotationSchema>;
```

---

## Integration with React Hook Form

```tsx
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { loginSchema, LoginFormData } from '@/lib/validations/auth';

const { register, handleSubmit, formState: { errors } } = useForm<LoginFormData>({
  resolver: zodResolver(loginSchema),
});
```

Validation error messages use localization keys — they are resolved to the active locale via `useTranslations()` before displaying.

---

## Two-Layer Validation Strategy

| Layer | Tool | Purpose |
|---|---|---|
| **Client-side** | Zod + React Hook Form | Instant feedback, prevents unnecessary API calls |
| **Server-side** | FluentValidation + MediatR `ValidationBehavior` | Authoritative validation, returns errors via `ApiResponse.errors` |

### Client-side (Zod)
- Validates form fields before submission.
- Shows inline errors under each field.
- Disables submit button until the form is valid.

### Server-side (`ApiResponse.errors`)
- If the backend returns `isSuccessful: false` with `errors`, each error is displayed as a toast.
- This handles edge cases like duplicate emails, race conditions, or business rules that can't be checked client-side.

---

## Localization Keys for Validation

Add these keys to `messages/tr.json` and `messages/en.json`:

```json
{
  "errors": {
    "required": "Bu alan zorunludur",
    "invalidEmail": "Geçerli bir e-posta giriniz",
    "generic": "Bir hata oluştu"
  },
  "admin": {
    "validation": {
      "priceRequired": "Birim fiyat 0'dan büyük olmalıdır",
      "discountMin": "İndirim 0'dan küçük olamaz",
      "discountMax": "İndirim 100'den büyük olamaz",
      "atLeastOneItem": "En az bir ürün gereklidir"
    }
  }
}
```
