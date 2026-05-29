"use client";

import { useCallback, useEffect, useState } from "react";
import { useTranslations } from "next-intl";
import { requestsApi, RequestListItemDto } from "@/lib/api/requests";
import { useRouter, useParams } from "next/navigation";
import { formatPrice } from "@/lib/utils";
import { Spinner } from "@/components/ui/Spinner";
import { useAuth } from "@/context/AuthContext";
import { Pagination } from "@/components/common/Pagination";

export default function AdminRequestsPage() {
  const { role, accessToken, isLoading: authLoading } = useAuth();
  const t = useTranslations("AdminRequests");
  const tCommon = useTranslations("Common");
  const router = useRouter();
  const params = useParams();
  const locale = params.locale as string;

  const [requests, setRequests] = useState<RequestListItemDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [pageSize, setPageSize] = useState(10);
  
  const [sortColumn, setSortColumn] = useState<string>("CreatedAt");
  const [sortDirection, setSortDirection] = useState<string>("Desc");

  const fetchRequests = useCallback(
    async (pageIndex: number, currentSortCol: string, currentSortDir: string) => {
      try {
        setLoading(true);
        const res = await requestsApi.getAll(pageIndex, pageSize, currentSortCol, currentSortDir, accessToken);
        if (res.data) {
          setRequests(res.data.items);
          setTotalPages(res.data.totalPages);
          setTotalCount(res.data.totalCount);
        }
      } catch (error) {
        console.error("Failed to fetch requests", error);
      } finally {
        setLoading(false);
      }
    },
    [accessToken, pageSize],
  );

  useEffect(() => {
    if (role === "Admin") {
      void fetchRequests(page, sortColumn, sortDirection);
    }
  }, [fetchRequests, page, sortColumn, sortDirection, role]);

  const handleSort = (column: string) => {
    if (sortColumn === column) {
      setSortDirection(sortDirection === "Asc" ? "Desc" : "Asc");
    } else {
      setSortColumn(column);
      setSortDirection("Asc");
    }
    setPage(0);
  };

  const getStatusBadge = (status: number) => {
    switch (status) {
      case 0:
        return (
          <span className="px-2 py-1 bg-yellow-100 text-yellow-800 rounded-full text-xs font-medium">
            {t("StatusPending")}
          </span>
        );
      case 1:
        return (
          <span className="px-2 py-1 bg-green-100 text-green-800 rounded-full text-xs font-medium">
            {t("StatusSent")}
          </span>
        );
      case 2:
        return (
          <span className="px-2 py-1 bg-red-100 text-red-800 rounded-full text-xs font-medium">
            {t("StatusCancelled")}
          </span>
        );
      default:
        return <span>Unknown</span>;
    }
  };

  if (authLoading || (loading && requests.length === 0)) {
    return <div className="p-8 flex justify-center items-center h-[50vh]"><Spinner size="lg" /></div>;
  }

  if (role !== "Admin") {
    return (
      <div className="p-8 text-center text-red-600 font-semibold">
        {tCommon("UnauthorizedAdmin")}
      </div>
    );
  }

  const renderSortIcon = (column: string) => {
    if (sortColumn !== column) return <span className="ml-1 opacity-0 group-hover:opacity-50 transition-opacity">↕</span>;
    return sortDirection === "Asc" ? <span className="ml-1">↑</span> : <span className="ml-1">↓</span>;
  };

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-slate-900">{t("Title")}</h1>
      </div>

      <div className="bg-white rounded-xl shadow overflow-hidden border border-slate-200">
        <table className="min-w-full divide-y divide-slate-200">
          <thead className="bg-slate-50">
            <tr>
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
              <th 
                className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider cursor-pointer group hover:bg-slate-100 transition-colors"
                onClick={() => handleSort("CreatedAt")}
              >
                {t("Date")} {renderSortIcon("CreatedAt")}
              </th>
              <th 
                className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider cursor-pointer group hover:bg-slate-100 transition-colors"
                onClick={() => handleSort("TotalAmount")}
              >
                {t("Total")} {renderSortIcon("TotalAmount")}
              </th>
              <th 
                className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase tracking-wider cursor-pointer group hover:bg-slate-100 transition-colors"
                onClick={() => handleSort("Status")}
              >
                {t("Status")} {renderSortIcon("Status")}
              </th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-slate-200">
            {requests.map((req) => (
              <tr
                key={req.id}
                className="hover:bg-slate-50 cursor-pointer transition-colors"
                onClick={() =>
                  router.push(`/${locale}/admin/requests/${req.id}`)
                }
              >
                <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-slate-900">
                  {req.requestNo}
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  <div className="text-sm text-slate-900">
                    {req.customerName}
                  </div>
                  <div className="text-sm text-slate-500">
                    {req.customerEmail}
                  </div>
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm text-slate-500">
                  {new Date(req.createdAt).toLocaleDateString()}
                </td>
                <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-slate-900">
                  {formatPrice(req.totalAmount, req.currency)}
                </td>
                <td className="px-6 py-4 whitespace-nowrap">
                  {getStatusBadge(req.status)}
                </td>
              </tr>
            ))}
            {requests.length === 0 && (
              <tr>
                <td
                  colSpan={5}
                  className="px-6 py-12 text-center text-slate-500"
                >
                  {t("NoRequests")}
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
