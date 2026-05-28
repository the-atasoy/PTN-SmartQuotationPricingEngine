"use client";

import { useAuth } from "@/context/AuthContext";
import { useTranslations } from "next-intl";
import Link from "next/link";
import { ArrowLeft, Settings } from "lucide-react";

export function AdminClient() {
  const { role, isLoading } = useAuth();
  const t = useTranslations("nav");

  if (isLoading) {
    return <div className="p-8 text-center text-gray-500">Yükleniyor...</div>;
  }

  if (role !== "Admin") {
    return (
      <div className="p-8 text-center text-red-600">
        <p className="font-semibold">Bu sayfaya erişim yetkiniz yok.</p>
        <Link href="/products" className="text-blue-600 hover:underline text-sm mt-2 inline-block">
          Ürünlere Geri Dön
        </Link>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="flex items-center gap-4 mb-8">
        <Link
          href="/products"
          className="flex items-center gap-2 text-sm text-gray-600 hover:text-gray-900 transition-colors"
        >
          <ArrowLeft className="w-4 h-4" />
          Ürünlere Geri Dön
        </Link>
      </div>

      <div className="flex items-center gap-3 mb-6">
        <Settings className="w-6 h-6 text-gray-700" />
        <h1 className="text-2xl font-bold text-gray-900">{t("admin")}</h1>
      </div>

      <div className="bg-white rounded-xl shadow-sm border border-gray-100 p-8 text-center text-gray-500">
        <p>Yönetim paneli yakında burada olacak.</p>
      </div>
    </div>
  );
}
