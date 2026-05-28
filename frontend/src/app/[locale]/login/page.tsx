import { LoginForm } from "@/components/auth/LoginForm";
import { getTranslations } from "next-intl/server";

export async function generateMetadata({ params }: { params: Promise<{ locale: string }> }) {
  const { locale } = await params;
  const t = await getTranslations({ locale, namespace: "Auth" });
  return {
    title: t("loginTitle") || "Login - Smart Quotation",
  };
}

export default function LoginPage() {
  return (
    <div className="flex min-h-screen items-center justify-center bg-gradient-to-br from-slate-900 via-blue-950 to-slate-900 p-4">
      <div className="w-full max-w-md">
        {/* Logo / Brand */}
        <div className="text-center mb-8">
          <h2 className="text-3xl font-bold text-white tracking-tight">Akıllı Teklif</h2>
          <p className="mt-2 text-sm text-blue-300">Smart Quotation Pricing Engine</p>
        </div>

        {/* Card */}
        <div className="bg-white rounded-2xl shadow-2xl p-8">
          <h1 className="text-xl font-semibold text-gray-900 mb-6 text-center">Giriş Yap</h1>
          <LoginForm />
        </div>
      </div>
    </div>
  );
}
