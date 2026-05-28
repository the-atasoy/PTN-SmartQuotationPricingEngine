import { NextResponse } from "next/server";
import type { NextRequest } from "next/server";
import createIntlMiddleware from "next-intl/middleware";

const intlMiddleware = createIntlMiddleware({
  locales: ["tr", "en"],
  defaultLocale: "tr",
  localePrefix: "never",
  localeCookie: { name: "NEXT_LOCALE" }
});

export function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl;
  
  // Public paths that do not require authentication
  const publicPaths = ["/login"];
  const isPublicPath = publicPaths.some(p => pathname.startsWith(p) || pathname === p);

  const authMetaCookie = request.cookies.get("auth_meta");
  let role: string | null = null;
  let exp: number | null = null;

  if (authMetaCookie) {
    try {
      const parsed = JSON.parse(authMetaCookie.value);
      role = parsed.role ?? null;
      exp = parsed.exp ?? null;
    } catch {
      // Malformed cookie — treat as unauthenticated
      role = null;
      exp = null;
    }
  }

  // Check if token is expired
  const isExpired = exp ? (Date.now() / 1000) > exp : true;
  const isAuthenticated = !!role && !isExpired;

  // Unauthenticated user accessing any protected route -> redirect to /login
  if (!isAuthenticated && !isPublicPath) {
    return NextResponse.redirect(new URL(`/login`, request.url));
  }

  // Authenticated user accessing /login -> redirect to /products
  if (isAuthenticated && isPublicPath) {
    return NextResponse.redirect(new URL(`/products`, request.url));
  }

  // Authenticated `User` role accessing any `/admin` or `/admin/*` route -> redirect to /products
  if (isAuthenticated && role === "User") {
    // Check if path is /admin or starts with /admin/
    if (pathname === "/admin" || pathname.startsWith("/admin/")) {
      return NextResponse.redirect(new URL(`/products`, request.url));
    }
  }

  // Run next-intl middleware
  return intlMiddleware(request);
}

export const config = {
  // Skip all paths that should not be internationalized
  matcher: ['/((?!api|_next|.*\\..*).*)']
};
