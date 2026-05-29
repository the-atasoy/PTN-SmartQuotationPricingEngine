"use client";

import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { ProductCard } from "./ProductCard";
import { ProductGrid } from "./ProductGrid";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { useTranslations } from "next-intl";
import { API_ENDPOINTS, getApiUrl } from "@/lib/api-endpoints";

interface Product {
  id: string;
  name: string;
  basePrice: number;
  lastRequestPrice: number | null;
  lastRequestCurrency: number | null;
  lastRequestDate: string | null;
}

interface PaginatedResult {
  items: Product[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export function ProductsClient() {
  const { accessToken: token, isLoading: authLoading } = useAuth();
  const t = useTranslations("Products");
  const [productsData, setProductsData] = useState<PaginatedResult | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  const [currentPage, setCurrentPage] = useState(0);
  const pageSize = 10;

  useEffect(() => {
    if (!token) {
      return;
    }

    const fetchProducts = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const response = await fetch(`${getApiUrl(API_ENDPOINTS.PRODUCTS.GET_ALL)}?page=${currentPage}&pageSize=${pageSize}`, {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        const data = await response.json();

        if (response.ok && data.isSuccessful) {
          setProductsData(data.data);
        } else {
          setError(data.message || t("fetchFailed"));
        }
      } catch {
        setError(t("fetchError"));
      } finally {
        setIsLoading(false);
      }
    };

    fetchProducts();
  }, [token, currentPage, pageSize, t]);

  // While auth is being restored, show a loading state to avoid "unauthorized" flash
  if (authLoading) {
    return <div className="p-8 text-center text-gray-500">{t("loading")}</div>;
  }

  if (!token) {
    return (
      <div className="p-8 text-center text-red-600">
        <p className="font-semibold">{t("unauthorized")}</p>
      </div>
    );
  }

  if (isLoading) {
    return <div className="p-8 text-center text-gray-500">{t("loading")}</div>;
  }

  if (error) {
    return (
      <div className="p-8 text-center text-red-600">
        <p className="font-semibold">{error}</p>
      </div>
    );
  }

  if (!productsData || productsData.items.length === 0) {
    return <div className="p-8 text-center text-gray-500">{t("noProducts")}</div>;
  }

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="flex items-center justify-between mb-8">
        <h1 className="text-3xl font-bold text-gray-900">{t("title")}</h1>
      </div>

      <ProductGrid>
        {productsData.items.map((product) => (
          <ProductCard key={product.id} product={product} />
        ))}
      </ProductGrid>

      {/* Pagination Controls */}
      {productsData.totalPages > 1 && (
        <div className="mt-12 flex justify-center items-center gap-4">
          <button
            onClick={() => setCurrentPage((p) => Math.max(0, p - 1))}
            disabled={!productsData.hasPreviousPage}
            className="p-2 rounded-lg border border-gray-300 disabled:opacity-50 hover:bg-gray-50 cursor-pointer disabled:cursor-not-allowed"
          >
            <ChevronLeft className="w-5 h-5" />
          </button>
          
          <span className="text-gray-600 font-medium">
            {t("pageOf", { page: productsData.page + 1, total: productsData.totalPages })}
          </span>
          
          <button
            onClick={() => setCurrentPage((p) => Math.min(productsData.totalPages - 1, p + 1))}
            disabled={!productsData.hasNextPage}
            className="p-2 rounded-lg border border-gray-300 disabled:opacity-50 hover:bg-gray-50 cursor-pointer disabled:cursor-not-allowed"
          >
            <ChevronRight className="w-5 h-5" />
          </button>
        </div>
      )}
    </div>
  );
}
