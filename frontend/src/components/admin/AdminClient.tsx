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

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
        <Link 
          href="/admin/requests"
          className="bg-white p-6 rounded-xl shadow border border-slate-200 hover:border-indigo-300 hover:shadow-md transition-all group flex flex-col"
        >
          <h3 className="text-lg font-semibold text-slate-900 group-hover:text-indigo-600 transition-colors">
            Teklif Talepleri (Quotation Requests)
          </h3>
          <p className="text-sm text-slate-500 mt-2 flex-grow">
            Müşterilerden gelen teklif taleplerini yönetin, fiyatlandırın ve müşterilere gönderin.
          </p>
          <div className="mt-4 text-sm font-medium text-indigo-600 group-hover:text-indigo-700">
            Yönetime Git &rarr;
          </div>
        </Link>
      </div>
    </div>
  );
}
