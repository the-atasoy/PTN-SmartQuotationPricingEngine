"use client";

import { useCallback, useEffect, useState } from "react";
import { useTranslations } from "next-intl";
import { useParams } from "next/navigation";
import { formatPrice } from "@/lib/utils";
import { useAuth } from "@/context/AuthContext";
import { productsApi } from "@/lib/api/products";
import { PriceHistoryDto } from "@/lib/types/api";
import { Spinner } from "@/components/ui/Spinner";
import { Pagination } from "@/components/common/Pagination";

export default function ProductPriceHistoryPage() {
  const { accessToken } = useAuth();
  const t = useTranslations("AdminProducts");
  const tCommon = useTranslations("Common");
  const params = useParams();
  const productId = params.id as string;

  const [history, setHistory] = useState<PriceHistoryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  
  const [sortColumn, setSortColumn] = useState<string>("CreatedAt");
  const [sortDirection, setSortDirection] = useState<string>("Desc");

  const fetchHistory = useCallback(
    async (pageIndex: number, currentSortCol: string, currentSortDir: string) => {
      try {
        setLoading(true);
        const res = await productsApi.getPriceHistory(productId, pageIndex, pageSize, currentSortCol, currentSortDir, accessToken);

        if (res.isSuccessful) {
          setHistory(res.data.items);
          setTotalPages(res.data.totalPages);
          setTotalCount(res.data.totalCount);
        }
      } catch (error) {
        console.error("Failed to fetch price history", error);
      } finally {
        setLoading(false);
      }
    },
    [accessToken, pageSize, productId],
  );

  useEffect(() => {
    void fetchHistory(page, sortColumn, sortDirection);
  }, [fetchHistory, page, sortColumn, sortDirection, productId]);

  const handleSort = (column: string) => {
    if (sortColumn === column) {
      setSortDirection(sortDirection === "Asc" ? "Desc" : "Asc");
    } else {
      setSortColumn(column);
      setSortDirection("Asc");
    }
    setPage(0);
  };

  if (loading && history.length === 0) {
    return <div className="p-8 flex justify-center items-center h-[50vh]"><Spinner size="lg" /></div>;
  }

  const renderSortIcon = (column: string) => {
    if (sortColumn !== column) return <span className="ml-1 opacity-0 group-hover:opacity-50 transition-opacity">↕</span>;
    return sortDirection === "Asc" ? <span className="ml-1">↑</span> : <span className="ml-1">↓</span>;
  };

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="mb-6">
        <h1 className="text-3xl font-bold text-slate-900">
          {t("PriceHistoryTitle")}
        </h1>
        <p className="text-slate-500 mt-2">{t("PriceHistoryDesc")}</p>
      </div>

      <div className="bg-white rounded-xl shadow overflow-hidden border border-slate-200">
        <table className="min-w-full divide-y divide-slate-200">
          <thead className="bg-slate-50">
            <tr>
              <th 
                className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider cursor-pointer group hover:bg-slate-100 transition-colors"
                onClick={() => handleSort("CreatedAt")}
              >
                {t("Date")} {renderSortIcon("CreatedAt")}
              </th>
              <th 
                className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider cursor-pointer group hover:bg-slate-100 transition-colors"
                onClick={() => handleSort("Price")}
              >
                {t("QuotedPrice")} {renderSortIcon("Price")}
              </th>
              <th 
                className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider cursor-pointer group hover:bg-slate-100 transition-colors"
                onClick={() => handleSort("RequestNo")}
              >
                {t("RequestNo")} {renderSortIcon("RequestNo")}
              </th>
              <th 
                className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider cursor-pointer group hover:bg-slate-100 transition-colors"
                onClick={() => handleSort("CustomerName")}
              >
                {t("Customer")} {renderSortIcon("CustomerName")}
              </th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-slate-200">
            {history.map((item) => (
              <tr key={item.id} className="hover:bg-slate-50 transition-colors">
                <td className="px-6 py-4 whitespace-nowrap text-sm text-slate-500">
                  {new Date(item.createdAt).toLocaleString()}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-slate-900">
                  {formatPrice(item.price, item.currency)}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-slate-900">
                  {item.requestNo}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-slate-500">
                  {item.customerName}
                </td>
              </tr>
            ))}
            {history.length === 0 && (
              <tr>
                <td
                  colSpan={4}
                  className="px-6 py-12 text-center text-slate-500"
                >
                  {t("NoPriceHistory")}
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      {totalPages > 1 && (
        <Pagination
          page={page + 1}
          pageSize={pageSize}
          totalCount={totalCount}
          totalPages={totalPages}
          hasNextPage={page < totalPages - 1}
          hasPreviousPage={page > 0}
          onPageChange={(newPage) => setPage(newPage - 1)}
          onPageSizeChange={(newSize) => {
            setPageSize(newSize);
            setPage(0);
          }}
        />
      )}
    </div>
  );
}
