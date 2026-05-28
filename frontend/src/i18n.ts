import { notFound } from "next/navigation";
import { getRequestConfig } from "next-intl/server";

export const locales = ["tr", "en"] as const;
export const defaultLocale = "tr";

export type Locale = (typeof locales)[number];

export default getRequestConfig(async ({ locale }) => {
  const resolvedLocale = (locale ?? defaultLocale) as Locale;
  if (!locales.includes(resolvedLocale)) {
    notFound();
  }

  return {
    locale: resolvedLocale,
    messages: (await import(`../messages/${resolvedLocale}.json`)).default,
  };
});
