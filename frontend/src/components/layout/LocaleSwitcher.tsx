"use client";

import { useLocale } from "next-intl";
import { usePathname, useRouter } from "@/navigation";
import { useTransition } from "react";

export function LocaleSwitcher() {
  const locale = useLocale();
  const router = useRouter();
  const pathname = usePathname();
  const [isPending, startTransition] = useTransition();

  const switchLocale = () => {
    const newLocale = locale === "tr" ? "en" : "tr";
    startTransition(() => {
      router.replace(pathname, { locale: newLocale });
    });
  };

  return (
    <button
      onClick={switchLocale}
      disabled={isPending}
      className="text-gray-600 hover:text-gray-900 transition-colors text-sm font-medium cursor-pointer disabled:opacity-50"
    >
      {locale === "tr" ? "EN" : "TR"}
    </button>
  );
}
