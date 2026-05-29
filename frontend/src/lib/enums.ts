export enum Currency {
  TRY = 1,
  USD = 2,
  EUR = 3
}

export const formatCurrencyEnum = (currencyVal: number) => {
  switch (currencyVal) {
    case Currency.TRY: return 'TRY';
    case Currency.USD: return 'USD';
    case Currency.EUR: return 'EUR';
    default: return 'TRY';
  }
};
