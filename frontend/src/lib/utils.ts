/**
 * Formats a numeric amount as currency based on the enum mapped integer.
 * 0 -> TRY
 * 1 -> USD
 * 2 -> EUR
 */
export function formatCurrency(amount: number, currency: number, locale?: string): string {
  let currencyCode = 'USD';

  switch (currency) {
    case 0:
      currencyCode = 'TRY';
      break;
    case 1:
      currencyCode = 'USD';
      break;
    case 2:
      currencyCode = 'EUR';
      break;
    default:
      currencyCode = 'USD';
  }

  return new Intl.NumberFormat(locale, {
    style: 'currency',
    currency: currencyCode,
  }).format(amount);
}
