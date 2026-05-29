"use client";

import { useCart } from "@/context/CartContext";
import { useAuth } from "@/context/AuthContext";
import { PlusCircle } from "lucide-react";
import { useTranslations } from "next-intl";
import { formatPrice } from "@/lib/utils";
import { Button } from "@/components/ui/Button";

interface ProductCardProps {
  product: {
    id: string;
    name: string;
    basePrice: number;
    lastRequestPrice?: number | null;
    lastRequestCurrency?: number | null;
    lastRequestDate?: string | null;
  };
}

export function ProductCard({ product }: ProductCardProps) {
  const { addToCart } = useCart();
  const { role } = useAuth();
  const t = useTranslations("Products");

  return (
    <div className="bg-white rounded-xl shadow-sm border border-gray-100 overflow-hidden hover:shadow-md transition-shadow h-full">
      <div className="p-6 flex flex-col h-full">
        <h3 className="text-lg font-semibold text-gray-900 mb-2">{product.name}</h3>
        
        <div className="mt-4 flex items-baseline gap-2">
          <span className="text-2xl font-bold text-gray-900">
            ${product.basePrice.toFixed(2)}
          </span>
        </div>

        {role === "Admin" && product.lastRequestPrice != null && product.lastRequestDate && product.lastRequestCurrency != null && (
          <div className="mt-2 text-sm text-gray-500">
            {t("lastQuote")}: {formatPrice(product.lastRequestPrice, product.lastRequestCurrency)}
            <br />
            <span className="text-xs">
              {t("on")} {new Date(product.lastRequestDate!).toLocaleDateString()}
            </span>
          </div>
        )}

        {role !== "Admin" && (
          <div className="mt-auto pt-6">
            <Button
              variant="primary"
              onClick={() => addToCart({ id: product.id, name: product.name, price: product.basePrice })}
              className="w-full flex items-center justify-center gap-2"
            >
              <PlusCircle className="w-5 h-5" />
              {t("addToCart")}
            </Button>
          </div>
        )}
      </div>
    </div>
  );
}
