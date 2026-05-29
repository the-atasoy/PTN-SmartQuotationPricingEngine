"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations } from "next-intl";
import { loginSchema, LoginFormData } from "@/lib/validations/auth";
import { useAuth } from "@/context/AuthContext";
import { toast } from "sonner";
import { useRouter } from "next/navigation";
import { API_ENDPOINTS, getApiUrl } from "@/lib/api-endpoints";
import { Input } from "@/components/ui/Input";
import { Button } from "@/components/ui/Button";

export function LoginForm() {
  const t = useTranslations("Auth");
  const { login } = useAuth();
  const router = useRouter();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormData) => {
    try {
      setIsSubmitting(true);
      const res = await fetch(getApiUrl(API_ENDPOINTS.AUTH.LOGIN), {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(data),
      });

      const responseData = await res.json();

      if (responseData.isSuccessful && responseData.data?.accessToken) {
        const token = responseData.data.accessToken;
        try {
          const payload = JSON.parse(atob(token.split(".")[1]));
          const emailClaim = payload.email || payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"] || "";
          login(token, payload.role, emailClaim);
        } catch {
          login(token, "User", ""); // fallback if payload decode fails
        }
        toast.success(t("loginSuccess") || "Logged in successfully");
        router.push("/products");
      } else {
        toast.error(responseData.message || t("loginFailed") || "Login failed");
      }
    } catch {
      toast.error(t("loginError") || "An unexpected error occurred");
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
      <Input
        type="email"
        label={t("email") || "Email"}
        {...register("email")}
        placeholder="ornek@sirket.com"
        error={errors.email?.message}
      />

      <Input
        type="password"
        label={t("password") || "Password"}
        {...register("password")}
        placeholder="••••••••"
        error={errors.password?.message}
      />

      <Button
        type="submit"
        isLoading={isSubmitting}
        className="w-full mt-2"
      >
        {isSubmitting ? (t("loggingIn") || "Logging in...") : (t("login") || "Login")}
      </Button>
    </form>
  );
}
