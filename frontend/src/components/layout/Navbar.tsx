"use client";

import Link from "next/link";
import { useTranslations } from "next-intl";
import { useCart } from "@/context/CartContext";
import { useAuth } from "@/context/AuthContext";
import { ShoppingCart } from "lucide-react";
import { LocaleSwitcher } from "./LocaleSwitcher";

export function Navbar() {
  const t = useTranslations("nav");
  const { totalItems } = useCart();
  const { role, logout } = useAuth();

  return (
    <nav className="bg-white border-b border-gray-200">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex justify-between h-16 items-center">
          <div className="flex shrink-0 items-center">
            <Link href="/" className="text-xl font-bold text-gray-900">
              {t("title")}
            </Link>
          </div>
          <div className="flex items-center space-x-4">
            <Link href="/products" className="text-gray-600 hover:text-gray-900 transition-colors">
              {t("products")}
            </Link>
            {role === "Admin" && (
              <Link href="/admin" className="text-gray-600 hover:text-gray-900 transition-colors">
                {t("admin")}
              </Link>
            )}
            {role !== "Admin" && (
              <div className="relative p-2">
                <ShoppingCart className="w-6 h-6 text-gray-600" />
                {totalItems > 0 && (
                  <span className="absolute top-0 right-0 inline-flex items-center justify-center px-2 py-1 text-xs font-bold leading-none text-white transform translate-x-1/4 -translate-y-1/4 bg-red-600 rounded-full">
                    {totalItems}
                  </span>
                )}
              </div>
            )}
            <LocaleSwitcher />
            {role && (
              <button
                onClick={logout}
                className="text-gray-600 hover:text-gray-900 transition-colors"
              >
                {t("logout")}
              </button>
            )}
          </div>
        </div>
      </div>
    </nav>
  );
}
