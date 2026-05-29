"use client";

import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { Spinner } from "@/components/ui/Spinner";
import { productsApi } from "@/lib/api/products";
import { Product, PaginatedResult } from "@/lib/types/api";
import { Pagination } from "@/components/common/Pagination";
import { useTranslations } from "next-intl";
import { Input } from "@/components/ui/Input";

export function ProductsClient() {
  const { accessToken: token, isLoading: authLoading } = useAuth();
  const t = useTranslations("Products");
  const [productsData, setProductsData] = useState<PaginatedResult<Product> | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(true);
  
  const [currentPage, setCurrentPage] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  
  const [searchTerm, setSearchTerm] = useState("");
  const [debouncedSearch, setDebouncedSearch] = useState("");
  const [sortColumn, setSortColumn] = useState<string>("Name");
  const [sortDirection, setSortDirection] = useState<string>("Asc");

  // Debounce search term
  useEffect(() => {
    const timer = setTimeout(() => setDebouncedSearch(searchTerm), 500);
    return () => clearTimeout(timer);
  }, [searchTerm]);

  // Reset page when search changes
  useEffect(() => {
    setCurrentPage(0);
  }, [debouncedSearch]);

  const handleSort = (column: string) => {
    if (sortColumn === column) {
      setSortDirection(sortDirection === "Asc" ? "Desc" : "Asc");
    } else {
      setSortColumn(column);
      setSortDirection("Asc");
    }
    setCurrentPage(0);
  };

  useEffect(() => {
    if (!token) return;

    const fetchProducts = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const response = await productsApi.getAll(
          currentPage, 
          pageSize, 
          debouncedSearch,
          sortColumn,
          sortDirection,
          token
        );

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
  }, [token, currentPage, pageSize, debouncedSearch, sortColumn, sortDirection, t]);

  if (authLoading) return <div className="p-8 flex justify-center items-center"><Spinner size="lg" /></div>;
  if (!token) return <div className="p-8 text-center text-red-600"><p className="font-semibold">{t("unauthorized")}</p></div>;

  const renderSortIcon = (column: string) => {
    if (sortColumn !== column) return <span className="ml-1 text-slate-300 opacity-0 group-hover:opacity-100 transition-opacity">↕</span>;
    return sortDirection === "Asc" ? <span className="ml-1 text-slate-600">↑</span> : <span className="ml-1 text-slate-600">↓</span>;
  };

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8 animate-in fade-in slide-in-from-bottom-4 duration-500">
      <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between mb-8 gap-4">
        <div>
          <h1 className="text-3xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-slate-900 to-slate-600">
            {t("title")}
          </h1>
          <p className="text-slate-500 mt-1">Browse and search through our product catalog.</p>
        </div>
        
        <div className="w-full sm:w-72">
          <Input 
            placeholder="Search products..." 
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="w-full bg-white shadow-sm border-slate-200 focus:ring-slate-500 focus:border-slate-500 rounded-full px-4"
          />
        </div>
      </div>

      <div className="bg-white/60 backdrop-blur-md rounded-2xl shadow-sm border border-slate-200/60 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full text-left border-collapse">
            <thead>
              <tr className="bg-slate-50/50 border-b border-slate-200">
                <th 
                  className="px-6 py-4 text-sm font-semibold text-slate-600 cursor-pointer group hover:bg-slate-100/50 transition-colors w-1/2"
                  onClick={() => handleSort("Name")}
                >
                  <div className="flex items-center">
                    Product Name {renderSortIcon("Name")}
                  </div>
                </th>
                <th 
                  className="px-6 py-4 text-sm font-semibold text-slate-600 cursor-pointer group hover:bg-slate-100/50 transition-colors w-1/2"
                  onClick={() => handleSort("BasePrice")}
                >
                  <div className="flex items-center">
                    Base Price {renderSortIcon("BasePrice")}
                  </div>
                </th>
              </tr>
            </thead>
            <tbody className="divide-y divide-slate-100">
              {isLoading && !productsData ? (
                <tr>
                  <td colSpan={2} className="px-6 py-12 text-center">
                    <div className="flex justify-center"><Spinner /></div>
                  </td>
                </tr>
              ) : error ? (
                <tr>
                  <td colSpan={2} className="px-6 py-8 text-center text-red-500 font-medium">
                    {error}
                  </td>
                </tr>
              ) : productsData?.items.length === 0 ? (
                <tr>
                  <td colSpan={2} className="px-6 py-12 text-center text-slate-500">
                    No products found matching your search.
                  </td>
                </tr>
              ) : (
                productsData?.items.map((product) => (
                  <tr 
                    key={product.id} 
                    className="hover:bg-slate-50/80 transition-colors duration-200 group"
                  >
                    <td className="px-6 py-4">
                      <div className="font-medium text-slate-900 group-hover:text-slate-700 transition-colors">
                        {product.name}
                      </div>
                    </td>
                    <td className="px-6 py-4 text-slate-600">
                      ${product.basePrice.toLocaleString(undefined, { minimumFractionDigits: 2 })}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>

      {productsData && productsData.totalPages > 1 && (
        <div className="mt-8">
          <Pagination
            page={currentPage + 1}
            pageSize={pageSize}
            totalCount={productsData.totalCount}
            totalPages={productsData.totalPages}
            hasNextPage={productsData.hasNextPage}
            hasPreviousPage={productsData.hasPreviousPage}
            onPageChange={(newPage) => setCurrentPage(newPage - 1)}
            onPageSizeChange={(newSize) => {
              setPageSize(newSize);
              setCurrentPage(0);
            }}
          />
        </div>
      )}
    </div>
  );
}
