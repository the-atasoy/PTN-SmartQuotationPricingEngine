"use client";

import { useLocale } from "next-intl";
import { useRouter } from "next/navigation";

export function LocaleSwitcher() {
  const locale = useLocale();
  const router = useRouter();

  const switchLocale = () => {
    const newLocale = locale === "tr" ? "en" : "tr";
    document.cookie = `NEXT_LOCALE=${newLocale}; path=/; max-age=31536000; samesite=lax`;
    router.refresh();
  };

  return (
    <button
      onClick={switchLocale}
      className="text-gray-600 hover:text-gray-900 transition-colors text-sm font-medium"
    >
      {locale === "tr" ? "EN" : "TR"}
    </button>
  );
}
