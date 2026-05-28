"use client";

import { useLocale } from "next-intl";

export function LocaleSwitcher() {
  const locale = useLocale();

  const switchLocale = () => {
    const newLocale = locale === "tr" ? "en" : "tr";
    document.cookie = `NEXT_LOCALE=${newLocale}; path=/; max-age=31536000; samesite=lax`;
    window.location.reload();
  };

  return (
    <button
      onClick={switchLocale}
      className="text-gray-600 hover:text-gray-900 transition-colors text-sm font-medium cursor-pointer"
    >
      {locale === "tr" ? "EN" : "TR"}
    </button>
  );
}
