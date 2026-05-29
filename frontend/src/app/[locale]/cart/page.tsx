"use client";

import { useState } from "react";
import { useTranslations } from "next-intl";
import { useCart } from "@/context/CartContext";
import { useAuth } from "@/context/AuthContext";
import { Trash2, Plus, Minus } from "lucide-react";
import { Link } from "@/navigation";
import { toast } from "sonner";
import { getApiUrl, API_ENDPOINTS } from "@/lib/api-endpoints";
import { Currency } from "@/lib/enums";
import { Button } from "@/components/ui/Button";

export default function CartPage() {
  const t = useTranslations("cart");
  const tNav = useTranslations("nav");
  const { items, removeFromCart, updateQuantity, totalItems, clearCart } = useCart();
  const { email, accessToken } = useAuth();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [selectedCurrency, setSelectedCurrency] = useState(Currency.TRY);

  const onSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!email) {
      toast.error(t("loginRequired") || "Please login to request a quotation.");
      return;
    }

    try {
      setIsSubmitting(true);
      
      const payload = {
        customerEmail: email,
        currency: selectedCurrency,
        items: items.map(item => ({
          productId: item.productId,
          quantity: item.quantity
        }))
      };

      const res = await fetch(getApiUrl(API_ENDPOINTS.REQUESTS.BASE), {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          ...(accessToken ? { "Authorization": `Bearer ${accessToken}` } : {})
        },
        body: JSON.stringify(payload),
      });

      const responseData = await res.json();

      if (res.ok && responseData.isSuccessful) {
        toast.success(t("requestSuccess") || "Quotation request submitted successfully.");
        clearCart();
      } else {
        if (responseData.errors && responseData.errors.length > 0) {
          responseData.errors.forEach((err: string) => toast.error(err));
        } else {
          toast.error(responseData.message || t("requestFailed") || "Failed to submit request.");
        }
      }
    } catch {
      toast.error(t("unexpectedError") || "An unexpected error occurred.");
    } finally {
      setIsSubmitting(false);
    }
  };

  if (totalItems === 0) {
    return (
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12 flex flex-col items-center justify-center min-h-[60vh]">
        <div className="text-gray-400 mb-4">
          <svg className="w-16 h-16 mx-auto" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1.5} d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
          </svg>
        </div>
        <h2 className="text-2xl font-bold text-gray-900 mb-2">{t("emptyCart")}</h2>
        <Link href="/products" className="mt-6 px-6 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors font-medium">
          {tNav("products")}
        </Link>
      </div>
    );
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-12">
      <h1 className="text-3xl font-bold text-gray-900 mb-8">{tNav("cart")}</h1>
      
      <div className="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <div className="lg:col-span-2">
          <div className="bg-white shadow-sm rounded-xl border border-gray-200 overflow-hidden">
            <ul className="divide-y divide-gray-200">
              {items.map((item) => (
                <li key={item.productId} className="p-6 flex flex-col sm:flex-row sm:items-center justify-between hover:bg-gray-50 transition-colors gap-4">
                  <div className="flex-1">
                    <h3 className="text-lg font-medium text-gray-900">{item.name}</h3>
                  </div>
                  
                  <div className="flex items-center gap-6">
                    {/* Quantity Controls */}
                    <div className="flex items-center gap-3 bg-gray-50 border border-gray-200 rounded-lg p-1">
                      <button
                         onClick={() => updateQuantity(item.productId, item.quantity - 1)}
                        className="p-1.5 text-gray-500 hover:text-gray-700 hover:bg-gray-200 rounded-md transition-colors cursor-pointer"
                        disabled={isSubmitting}
                      >
                        <Minus className="w-4 h-4" />
                      </button>
                      <span className="w-8 text-center font-medium text-gray-900 text-sm">
                        {item.quantity}
                      </span>
                      <button
                        onClick={() => updateQuantity(item.productId, item.quantity + 1)}
                        className="p-1.5 text-gray-500 hover:text-gray-700 hover:bg-gray-200 rounded-md transition-colors cursor-pointer"
                        disabled={isSubmitting}
                      >
                        <Plus className="w-4 h-4" />
                      </button>
                    </div>

                    <button
                      onClick={() => removeFromCart(item.productId)}
                      className="p-2 text-gray-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-colors cursor-pointer"
                      title="Remove"
                      disabled={isSubmitting}
                    >
                      <Trash2 className="w-5 h-5" />
                    </button>
                  </div>
                </li>
              ))}
            </ul>
          </div>
        </div>

        <div className="lg:col-span-1">
          <div className="bg-white shadow-sm rounded-xl border border-gray-200 p-6 sticky top-20">
            <h2 className="text-lg font-medium text-gray-900 mb-6">{t("requestQuote")}</h2>
            
            <form onSubmit={onSubmit} className="space-y-5">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1.5">
                  Email
                </label>
                <div className="w-full px-4 py-2 bg-gray-50 border border-gray-200 rounded-lg text-gray-600 break-all select-all font-medium text-sm">
                  {email}
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1.5">
                  Currency
                </label>
                <select
                  value={selectedCurrency}
                  onChange={(e) => setSelectedCurrency(Number(e.target.value))}
                  className="w-full px-4 py-2 bg-white border border-gray-200 rounded-lg text-gray-900 focus:ring-blue-500 focus:border-blue-500"
                >
                  <option value={Currency.TRY}>TRY</option>
                  <option value={Currency.USD}>USD</option>
                  <option value={Currency.EUR}>EUR</option>
                </select>
              </div>



              <div className="pt-4 border-t border-gray-100">
                <Button
                  type="submit"
                  isLoading={isSubmitting}
                  className="w-full"
                >
                  {t("requestQuote")}
                </Button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}
