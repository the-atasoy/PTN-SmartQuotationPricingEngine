"use client";

import Link from "next/link";
import { useTranslations } from "next-intl";
import { useCart } from "@/context/CartContext";
import { useAuth } from "@/context/AuthContext";
import { ShoppingCart, LogOut } from "lucide-react";
import { LocaleSwitcher } from "./LocaleSwitcher";

export function Navbar() {
  const t = useTranslations("nav");
  const { totalItems } = useCart();
  const { role, logout, isLoading } = useAuth();

  return (
    <nav className="bg-white border-b border-gray-200 shadow-sm">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="flex items-center justify-between h-14">
          {/* Brand */}
          <div className="flex-shrink-0">
            <Link
              href="/"
              className="text-lg font-bold text-gray-900 tracking-tight hover:text-blue-600 transition-colors"
            >
              {t("title")}
            </Link>
          </div>

          {/* Center nav links */}
          <div className="flex items-center gap-6">
            <Link
              href="/products"
              className="text-sm font-medium text-gray-600 hover:text-gray-900 transition-colors"
            >
              {t("products")}
            </Link>
            {!isLoading && role === "Admin" && (
              <Link
                href="/admin"
                className="text-sm font-medium text-gray-600 hover:text-gray-900 transition-colors"
              >
                {t("admin")}
              </Link>
            )}
          </div>

          {/* Right side actions */}
          <div className="flex items-center gap-3">
            {!isLoading && role !== "Admin" && (
              <div className="relative p-1.5">
                <ShoppingCart className="w-5 h-5 text-gray-600" />
                {totalItems > 0 && (
                  <span className="absolute -top-0.5 -right-0.5 inline-flex items-center justify-center w-4 h-4 text-[10px] font-bold text-white bg-red-500 rounded-full">
                    {totalItems}
                  </span>
                )}
              </div>
            )}

            <LocaleSwitcher />

            <button
              onClick={logout}
              className="flex items-center gap-1.5 text-sm font-medium text-gray-600 hover:text-red-600 transition-colors cursor-pointer"
              title={t("logout")}
            >
              <LogOut className="w-4 h-4" />
              <span className="hidden sm:inline">{t("logout")}</span>
            </button>
          </div>
        </div>
      </div>
    </nav>
  );
}
