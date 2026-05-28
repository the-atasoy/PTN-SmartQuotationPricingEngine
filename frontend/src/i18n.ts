import { getRequestConfig } from "next-intl/server";
import { routing } from "./routing";

export const locales = routing.locales;
export const defaultLocale = routing.defaultLocale;

export type Locale = (typeof routing.locales)[number];

export default getRequestConfig(async ({ requestLocale }) => {
  // requestLocale is async in next-intl v4
  let locale = await requestLocale;

  // Fall back to default if locale is not valid
  if (!locale || !routing.locales.includes(locale as Locale)) {
    locale = routing.defaultLocale;
  }

  return {
    locale,
    messages: (await import(`../messages/${locale}.json`)).default,
  };
});
