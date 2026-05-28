"use client";

import React, { createContext, useContext, useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { useLocale } from "next-intl";

interface AuthContextType {
  accessToken: string | null;
  role: string | null;
  login: (token: string, role: string, exp: number) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [accessToken, setAccessToken] = useState<string | null>(null);
  const [role, setRole] = useState<string | null>(null);
  const router = useRouter();
  const locale = useLocale();

  const login = React.useCallback((token: string, newRole: string, exp: number) => {
    setAccessToken(token);
    setRole(newRole);
    // auth_meta used by middleware
    document.cookie = `auth_meta=${JSON.stringify({ role: newRole, exp })}; path=/; max-age=604800; samesite=strict`;
  }, []);

  const logout = React.useCallback(async () => {
    try {
      await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/auth/logout`, {
        method: "POST",
        headers: { "Authorization": `Bearer ${accessToken}` },
        credentials: "include"
      });
    } catch (e) {
      console.error(e);
    }
    setAccessToken(null);
    setRole(null);
    document.cookie = "auth_meta=; path=/; expires=Thu, 01 Jan 1970 00:00:00 GMT";
    router.push(`/${locale}/login`);
  }, [accessToken, router, locale]);

  useEffect(() => {
    // Attempt to restore session on load (Task-011 criteria)
    // We don't have the access token stored. We only know if they should be logged in from auth_meta.
    // So we can hit /api/auth/refresh if auth_meta exists.
    const restoreSession = async () => {
      const hasAuthMeta = document.cookie.includes("auth_meta");
      if (hasAuthMeta && !accessToken) {
        try {
          const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/auth/refresh`, {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            credentials: "include",
            body: JSON.stringify({ accessToken: "dummy" }),
          });
          const data = await res.json();
          if (data.isSuccessful && data.data?.accessToken) {
            const token = data.data.accessToken;
            // Decode simple jwt payload
            const payload = JSON.parse(atob(token.split(".")[1]));
            login(token, payload.role, payload.exp);
          }
        } catch (e) {
          console.error("Failed to restore session", e);
        }
      }
    };
    restoreSession();
  }, [accessToken, login]);

  return (
    <AuthContext.Provider value={{ accessToken, role, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider");
  }
  return context;
};
