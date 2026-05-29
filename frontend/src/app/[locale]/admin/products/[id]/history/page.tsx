"use client";

import { useCallback, useEffect, useState } from "react";
import { useTranslations } from "next-intl";
import { useParams } from "next/navigation";
import { formatPrice } from "@/lib/utils";
import { useAuth } from "@/context/AuthContext";
import { productsApi } from "@/lib/api/products";
import { PriceHistoryDto } from "@/lib/types/api";
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
  const pageSize = 10;

  const fetchHistory = useCallback(
    async (pageIndex: number) => {
      try {
        setLoading(true);
        const res = await productsApi.getPriceHistory(productId, pageIndex, pageSize, accessToken);

        if (res.isSuccessful) {
          setHistory(res.data.items);
          setTotalPages(res.data.totalPages);
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
    // eslint-disable-next-line react-hooks/set-state-in-effect
    void fetchHistory(page);
  }, [fetchHistory, page, productId]);

  if (loading && history.length === 0) {
    return <div className="p-8 text-center">{tCommon("Loading")}</div>;
  }

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
              <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">
                {t("Date")}
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">
                {t("QuotedPrice")}
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">
                {t("RequestNo")}
              </th>
              <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider">
                {t("Customer")}
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
          totalPages={totalPages}
          hasNextPage={page < totalPages - 1}
          hasPreviousPage={page > 0}
          onPageChange={(newPage) => setPage(newPage - 1)}
        />
      )}
    </div>
  );
}
