# Frontend — Cart & State Management

## Cart Context

```ts
interface CartItem {
  productId: string;
  name: string;
  category: string;
  quantity: number;
  basePrice: number;
}

interface CartState {
  items: CartItem[];
  addItem: (product: Product) => void;
  removeItem: (productId: string) => void;
  updateQuantity: (productId: string, quantity: number) => void;
  clear: () => void;
}
```

Cart state lives in React context + `sessionStorage` for persistence across navigation (not `localStorage` — cleared when the tab closes).

---

## State Management Approach

This project uses **React Context** for state management — no external state library needed.

### Contexts

| Context | Purpose |
|---|---|
| `AuthContext` | Stores access token, user role, login/logout functions |
| `CartContext` | Stores cart items, add/remove/update/clear functions |

### Persistence Strategy

| State | Storage | Reason |
|---|---|---|
| Access token | Memory only (React state) | Security — never persisted to disk |
| Cart items | `sessionStorage` | Survives navigation, cleared on tab close |
| Refresh token | httpOnly cookie | Server-managed, inaccessible to JS |
| Auth metadata | Non-httpOnly cookie (`auth_meta`) | Enables edge middleware routing decisions |
