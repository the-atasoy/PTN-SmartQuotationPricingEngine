/**
 * Formats a numeric amount as TRY currency.
 */
export function formatPrice(amount: number, locale?: string): string {
  return new Intl.NumberFormat(locale, {
    style: 'currency',
    currency: 'TRY',
  }).format(amount);
}
