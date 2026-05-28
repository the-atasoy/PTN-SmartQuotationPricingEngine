import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";
import createIntlMiddleware from "next-intl/middleware";

const intlMiddleware = createIntlMiddleware({
  locales: ["tr", "en"],
  defaultLocale: "tr"
});

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;
  
  // Public paths that do not require authentication
  // Adjust based on your locales and public paths
  const publicPaths = ["/login", "/en/login", "/tr/login"];
  const isPublicPath = publicPaths.some(p => pathname.startsWith(p) || pathname === p);

  const authMetaCookie = request.cookies.get("auth_meta");
  let role: string | null = null;
  let exp: number | null = null;

  if (authMetaCookie) {
    try {
      const parsed = JSON.parse(authMetaCookie.value);
      role = parsed.role;
      exp = parsed.exp;
    } catch (e) {
      console.error("Failed to parse auth_meta cookie", e);
    }
  }

  // Check if token is expired
  const isExpired = exp ? (Date.now() / 1000) > exp : true;
  const isAuthenticated = !!role && !isExpired;

  // Unauthenticated user accessing any protected route -> redirect to /[locale]/login
  if (!isAuthenticated && !isPublicPath) {
    // Assuming English as fallback if locale is not detected properly in path yet
    const localeMatch = pathname.match(/^\/(en|tr)/);
    const locale = localeMatch ? localeMatch[1] : "tr";
    return NextResponse.redirect(new URL(`/${locale}/login`, request.url));
  }

  // Authenticated user accessing /login -> redirect to /[locale]/products
  if (isAuthenticated && isPublicPath) {
    const localeMatch = pathname.match(/^\/(en|tr)/);
    const locale = localeMatch ? localeMatch[1] : "tr";
    return NextResponse.redirect(new URL(`/${locale}/products`, request.url));
  }

  // Authenticated `User` role accessing any `/admin/*` route -> redirect to /[locale]/products
  if (isAuthenticated && role === "User") {
    // Check if path contains /admin/
    if (pathname.match(/^\/(en|tr)\/admin(\/|$)/) || pathname.startsWith("/admin/")) {
      const localeMatch = pathname.match(/^\/(en|tr)/);
      const locale = localeMatch ? localeMatch[1] : "tr";
      return NextResponse.redirect(new URL(`/${locale}/products`, request.url));
    }
  }

  // Run next-intl middleware
  return intlMiddleware(request);
}

export const config = {
  // Skip all paths that should not be internationalized
  matcher: ['/((?!api|_next|.*\\..*).*)']
};
