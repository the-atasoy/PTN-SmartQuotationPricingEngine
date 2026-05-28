"use client";

import { useCart } from "@/context/CartContext";
import { PlusCircle } from "lucide-react";
import { useTranslations } from "next-intl";

interface ProductCardProps {
  product: {
    id: string;
    name: string;
    basePrice: number;
    lastRequestPrice?: number | null;
    lastRequestDate?: string | null;
  };
}

export function ProductCard({ product }: ProductCardProps) {
  const { addToCart } = useCart();
  const t = useTranslations("Products");

  return (
    <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden hover:shadow-md transition-shadow">
      <div className="p-6">
        <h3 className="text-lg font-semibold text-gray-900 mb-2">{product.name}</h3>
        
        <div className="mt-4 flex items-baseline gap-2">
          <span className="text-2xl font-bold text-gray-900">
            ${product.basePrice.toFixed(2)}
          </span>
        </div>

        {product.lastRequestPrice != null && product.lastRequestDate && (
          <div className="mt-2 text-sm text-gray-500">
            {t("lastQuote")}: ${product.lastRequestPrice.toFixed(2)}
            <br />
            <span className="text-xs">
              {t("on")} {new Date(product.lastRequestDate!).toLocaleDateString()}
            </span>
          </div>
        )}

        <button
          onClick={() => addToCart({ id: product.id, name: product.name, price: product.basePrice })}
          className="mt-6 w-full flex items-center justify-center gap-2 bg-blue-600 hover:bg-blue-700 text-white py-2 px-4 rounded-lg transition-colors font-medium"
        >
          <PlusCircle className="w-5 h-5" />
          {t("addToCart")}
        </button>
      </div>
    </div>
  );
}
