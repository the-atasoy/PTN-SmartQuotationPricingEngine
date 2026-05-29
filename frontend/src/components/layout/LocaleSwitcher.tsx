"use client";

import { useLocale } from "next-intl";
import { usePathname, useRouter } from "@/navigation";
import { useTransition } from "react";
import { Button } from "@/components/ui/Button";

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
    <Button
      variant="secondary"
      onClick={switchLocale}
      isLoading={isPending}
      className="text-xs px-2 py-1"
    >
      {locale === "tr" ? "EN" : "TR"}
    </Button>
  );
}
