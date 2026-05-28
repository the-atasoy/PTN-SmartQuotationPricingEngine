import { createNavigation } from "next-intl/navigation";
import { routing } from "./routing";

// Locale-aware navigation hooks (useRouter, usePathname, Link, redirect)
// Use these instead of next/navigation equivalents for locale support.
export const { Link, useRouter, usePathname, redirect } =
  createNavigation(routing);
