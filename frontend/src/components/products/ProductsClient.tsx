"use client";

import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { Spinner } from "@/components/ui/Spinner";
import { ProductCard } from "./ProductCard";
import { productsApi } from "@/lib/api/products";
import { Product, PaginatedResult } from "@/lib/types/api";
import { Pagination } from "@/components/common/Pagination";
import { useTranslations } from "next-intl";

export function ProductsClient() {
  const { accessToken: token, isLoading: authLoading } = useAuth();
  const t = useTranslations("Products");
  const [productsData, setProductsData] = useState<PaginatedResult<Product> | null>(null);
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
        const response = await productsApi.getAll(currentPage, pageSize, token);

        if (response.isSuccessful) {
          setProductsData(response.data);
        } else {
          setError(response.errors?.[0] || t("fetchFailed"));
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
    return <div className="p-8 flex justify-center items-center"><Spinner size="lg" /></div>;
  }

  if (!token) {
    return (
      <div className="p-8 text-center text-red-600">
        <p className="font-semibold">{t("unauthorized")}</p>
      </div>
    );
  }

  if (isLoading) {
    return <div className="p-8 flex justify-center items-center"><Spinner size="lg" /></div>;
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

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
        {productsData.items.map((product) => (
          <ProductCard key={product.id} product={product} />
        ))}
      </div>

      {/* Pagination Controls */}
      {productsData.totalPages > 1 && (
        <Pagination
          page={currentPage + 1}
          totalPages={productsData.totalPages}
          hasNextPage={productsData.hasNextPage}
          hasPreviousPage={productsData.hasPreviousPage}
          onPageChange={(newPage) => setCurrentPage(newPage - 1)}
        />
      )}
    </div>
  );
}
