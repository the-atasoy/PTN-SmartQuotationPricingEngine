import { Currency, formatCurrencyEnum } from "./enums";

/**
 * Formats a numeric amount using the provided currency.
 */
export function formatPrice(
  amount: number,
  currency: number | string = Currency.TRY,
  locale?: string,
): string {
  return new Intl.NumberFormat(locale, {
    style: "currency",
    currency: formatCurrencyEnum(currency),
  }).format(amount);
}
