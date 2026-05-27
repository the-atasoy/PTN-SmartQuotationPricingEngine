# Frontend — Localization

## Overview

Localization is handled by **`next-intl`**. Supported locales: `tr` (default), `en`.

Locale is stored in the URL path segment: `/tr/products`, `/en/products`.

---

## Configuration

### `i18n.ts`

Configures `next-intl` with supported locales and default locale.

### `next.config.ts`

Integrates `next-intl` plugin into the Next.js config.

### `LocaleSwitcher` Component

Located in the Navbar. Toggles locale and sets a `NEXT_LOCALE` cookie. Switching locale reflects immediately in the URL without a page reload.

---

## Message Files

### `messages/tr.json`

```json
{
  "nav": {
    "products": "Ürünler",
    "cart": "Sepet",
    "admin": "Yönetim",
    "logout": "Çıkış"
  },
  "products": {
    "addToCart": "Sepete Ekle",
    "category": "Kategori",
    "basePrice": "Baz Fiyat"
  },
  "cart": {
    "requestQuote": "Teklif Al",
    "emailPlaceholder": "E-posta adresiniz",
    "emptyCart": "Sepetiniz boş"
  },
  "admin": {
    "uploadExcel": "Excel Yükle",
    "sendQuotation": "Teklif İlet",
    "noPreviousPrice": "Henüz teklif değeri girilmemiş",
    "status": {
      "Pending": "Beklemede",
      "Sent": "Gönderildi",
      "Cancelled": "İptal"
    }
  },
  "errors": {
    "required": "Bu alan zorunludur",
    "invalidEmail": "Geçerli bir e-posta giriniz",
    "generic": "Bir hata oluştu"
  }
}
```

### `messages/en.json`

English translations for all keys above.

---

## Usage

All UI strings must use the `useTranslations()` hook — no hardcoded Turkish or English strings.

```tsx
const t = useTranslations('products');
// ...
<button>{t('addToCart')}</button>
```

---

## API Integration

The `Accept-Language` header is automatically set on all API requests from the active `next-intl` locale, so the backend returns error messages in the matching language.
