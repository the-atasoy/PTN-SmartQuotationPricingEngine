"use client";

import React, { createContext, useContext, useState, useEffect } from "react";
import { useRouter } from "next/navigation";
import { API_ENDPOINTS, getApiUrl } from "@/lib/api-endpoints";

interface AuthContextType {
  accessToken: string | null;
  role: string | null;
  email: string | null;
  isLoading: boolean;
  login: (token: string, role: string, email: string) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const [accessToken, setAccessToken] = useState<string | null>(null);
  const [role, setRole] = useState<string | null>(null);
  const [email, setEmail] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const router = useRouter();

  // auth_meta (role + exp) is now set as an HttpOnly cookie by the backend
  // so that JS cannot forge it. The frontend only keeps the access token in memory.
  const login = React.useCallback((token: string, newRole: string, newEmail: string) => {
    setAccessToken(token);
    setRole(newRole);
    setEmail(newEmail);
  }, []);

  const logout = React.useCallback(async () => {
    try {
      await fetch(getApiUrl(API_ENDPOINTS.AUTH.LOGOUT), {
        method: "POST",
        headers: { "Authorization": `Bearer ${accessToken}` },
        credentials: "include"
      });
    } catch {
      // Best-effort logout — clear state regardless
    }
    setAccessToken(null);
    setRole(null);
    setEmail(null);
    router.push("/login");
  }, [accessToken, router]);

  useEffect(() => {
    // Attempt to restore session on load via the HttpOnly refresh token cookie.
    // We only attempt this once (when there is no access token in memory).
    const restoreSession = async () => {
      try {
        const res = await fetch(getApiUrl(API_ENDPOINTS.AUTH.REFRESH), {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          credentials: "include"
        });
        if (!res.ok) return;
        const data = await res.json();
        if (data.isSuccessful && data.data?.accessToken) {
          const token = data.data.accessToken;
          // Decode the JWT payload to extract the role and email claims
          const payloadBase64 = token.split(".")[1];
          const payload = JSON.parse(atob(payloadBase64));
          // Email claim name might be "email" or "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
          const emailClaim = payload.email || payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"];
          login(token, payload.role, emailClaim);
        }
      } catch {
        // Network error or malformed token — leave user unauthenticated
      } finally {
        setIsLoading(false);
      }
    };
    restoreSession();
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []); // Run only on mount

  return (
    <AuthContext.Provider value={{ accessToken, role, email, isLoading, login, logout }}>
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
