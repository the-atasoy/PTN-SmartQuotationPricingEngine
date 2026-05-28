"use client";

import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useTranslations } from "next-intl";
import { loginSchema, LoginFormData } from "@/lib/validations/auth";
import { useAuth } from "@/context/AuthContext";
import { toast } from "sonner";
import { useRouter } from "next/navigation";
import { useLocale } from "next-intl";

export function LoginForm() {
  const t = useTranslations("Auth");
  const { login } = useAuth();
  const router = useRouter();
  const locale = useLocale();

  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
  } = useForm<LoginFormData>({
    resolver: zodResolver(loginSchema),
  });

  const onSubmit = async (data: LoginFormData) => {
    try {
      const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL}/api/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(data),
      });

      const responseData = await res.json();

      if (responseData.isSuccessful && responseData.data?.accessToken) {
        const token = responseData.data.accessToken;
        const payload = JSON.parse(atob(token.split(".")[1]));
        
        login(token, payload.role, payload.exp);
        toast.success(t("loginSuccess") || "Logged in successfully");
        router.push(`/${locale}/products`);
      } else {
        toast.error(responseData.message || t("loginFailed") || "Login failed");
      }
    } catch (error) {
      toast.error(t("loginFailed") || "An unexpected error occurred");
    }
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
      <div>
        <label className="block text-sm font-medium mb-1">{t("email") || "Email"}</label>
        <input
          type="email"
          {...register("email")}
          className="w-full rounded-md border p-2"
        />
        {errors.email && (
          <p className="text-red-500 text-sm mt-1">{errors.email.message}</p>
        )}
      </div>

      <div>
        <label className="block text-sm font-medium mb-1">{t("password") || "Password"}</label>
        <input
          type="password"
          {...register("password")}
          className="w-full rounded-md border p-2"
        />
        {errors.password && (
          <p className="text-red-500 text-sm mt-1">{errors.password.message}</p>
        )}
      </div>

      <button
        type="submit"
        disabled={isSubmitting}
        className="w-full bg-blue-600 text-white p-2 rounded-md hover:bg-blue-700 disabled:opacity-50"
      >
        {isSubmitting ? (t("loggingIn") || "Logging in...") : (t("login") || "Login")}
      </button>
    </form>
  );
}
