"use client";

import { useEffect, useState, useRef } from "react";
import { useTranslations } from "next-intl";
import { requestsApi, RequestDetailDto } from "@/lib/api/requests";
import { excelApi, ParsedExcelResultDto } from "@/lib/api/excel";
import { useRouter, useParams } from "next/navigation";
import { formatPrice } from "@/lib/utils";
import { toast } from "sonner";
import { formatCurrencyEnum } from "@/lib/enums";

import { useAuth } from "@/context/AuthContext";

export default function AdminRequestDetailPage() {
  const { role, accessToken, isLoading: authLoading } = useAuth();
  const t = useTranslations("AdminRequests");
  const tCommon = useTranslations("Common");
  const router = useRouter();
  const params = useParams();
  const requestId = params.id as string;
  const locale = params.locale as string;

  const [request, setRequest] = useState<RequestDetailDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [isUploading, setIsUploading] = useState(false);
  const [parsedItems, setParsedItems] = useState<ParsedExcelResultDto[]>([]);
  const [quotationItems, setQuotationItems] = useState<Record<string, { unitPrice: number; discount: number; lineTotal: number }>>({});
  const [isSubmitting, setIsSubmitting] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    if (role === "Admin") {
      fetchRequestDetail();
    }
  }, [requestId, role]);

  const fetchRequestDetail = async () => {
    try {
      setLoading(true);
      const res = await requestsApi.getById(requestId, accessToken);
      if (res.data) {
        setRequest(res.data);
        if (res.data.status === 0 && parsedItems.length === 0) {
          const initialParsed = res.data.items.map(i => ({
            requestNo: res.data.requestNo,
            productId: i.productId,
            productName: i.productName,
            quantity: i.quantity,
            hasPreviousPrice: i.lastRequestPrice != null,
            lastRequestPrice: i.lastRequestPrice,
            lastRequestDate: i.lastRequestDate
          }));
          setParsedItems(initialParsed);
          
          const initialPricing: Record<string, { unitPrice: number; discount: number; lineTotal: number }> = {};
          initialParsed.forEach(item => {
            const unitPrice = item.lastRequestPrice ?? 0;
            initialPricing[item.productId] = { unitPrice, discount: 0, lineTotal: unitPrice * item.quantity };
          });
          setQuotationItems(initialPricing);
        }
      }
    } catch (error) {
      console.error("Failed to fetch request detail", error);
    } finally {
      setLoading(false);
    }
  };

  const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    try {
      setIsUploading(true);
      const res = await excelApi.parseExcel(file, requestId, accessToken);
      if (res.data) {
        setParsedItems(res.data);
        const initialPricing: Record<string, { unitPrice: number; discount: number; lineTotal: number }> = {};
        res.data.forEach((item: any) => {
          const unitPrice = item.unitPrice ?? item.lastRequestPrice ?? 0;
          const discount = item.discount ?? 0;
          initialPricing[item.productId] = {
            unitPrice: unitPrice,
            discount: discount,
            lineTotal: (unitPrice - discount) * item.quantity
          };
        });
        setQuotationItems(initialPricing);
      } else {
        toast.error(res.errors?.join(", ") || t("ExcelParseFailed"));
      }
    } catch (error: any) {
      toast.error(error.message || t("ExcelParseFailed"));
      if (fileInputRef.current) fileInputRef.current.value = '';
    } finally {
      setIsUploading(false);
    }
  };

  const handlePriceOrDiscountChange = (productId: string, quantity: number, val: string, field: 'price' | 'discount') => {
    const num = parseFloat(val);
    const value = isNaN(num) ? 0 : num;
    
    setQuotationItems(prev => {
      const current = prev[productId] || { unitPrice: 0, discount: 0 };
      const newUnitPrice = field === 'price' ? value : current.unitPrice;
      const newDiscount = field === 'discount' ? value : current.discount;
      
      return {
        ...prev,
        [productId]: {
          unitPrice: newUnitPrice,
          discount: newDiscount,
          lineTotal: (newUnitPrice - newDiscount) * quantity
        }
      };
    });
  };

  const handleDownloadExcel = async () => {
    try {
      const blob = await requestsApi.downloadExcel(requestId, accessToken);
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = `Request_${request?.requestNo || requestId}.xlsx`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      document.body.removeChild(a);
    } catch (error) {
      toast.error(tCommon("generic"));
    }
  };

  const handleSubmitQuotation = async () => {
    if (!request) return;

    const payloadItems: any[] = [];
    let hasError = false;

    parsedItems.forEach(item => {
      const pricing = quotationItems[item.productId];
      
      if (pricing.unitPrice <= 0) {
        toast.error(`${t("InvalidPrice")}: ${item.productName}`);
        hasError = true;
      }

      payloadItems.push({
        productId: item.productId,
        unitPrice: pricing.unitPrice,
        discount: pricing.discount
      });
    });

    if (hasError || payloadItems.length === 0) return;

    try {
      setIsSubmitting(true);
      await requestsApi.sendQuotation(request.id, {
        requestId: request.id,
        items: payloadItems
      }, accessToken);
      toast.success(t("QuotationSent"));
      router.push(`/${locale}/admin`);
    } catch (error: any) {
      toast.error(error.response?.data?.message || error.response?.data?.errors?.join(", ") || tCommon("generic"));
    } finally {
      setIsSubmitting(false);
    }
  };

  if (authLoading || (loading && !request)) {
    return <div className="p-8 text-center">{tCommon("Loading")}</div>;
  }

  if (role !== "Admin") {
    return <div className="p-8 text-center text-red-600 font-semibold">{tCommon("UnauthorizedAdmin")}</div>;
  }

  if (!request) {
    return <div className="p-8 text-center">{t("NotFound")}</div>;
  }

  const isPending = request.status === 0;
  const grandTotal = Object.values(quotationItems).reduce((sum, item) => sum + item.lineTotal, 0);

  const currencyStr = formatCurrencyEnum(request.currency);

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-3xl font-bold text-slate-900">{t("RequestDetail")} - {request.requestNo}</h1>
        <button 
          onClick={handleDownloadExcel}
          className="bg-indigo-600 hover:bg-indigo-700 text-white px-4 py-2 rounded-md shadow-sm text-sm font-medium transition-colors cursor-pointer"
        >
          {t("DownloadOriginalExcel")}
        </button>
      </div>

      <div className="bg-white p-6 rounded-xl shadow border border-slate-200 mb-8">
        <h2 className="text-lg font-semibold text-slate-900 mb-4">{t("CustomerInfo")}</h2>
        <div className="grid grid-cols-2 gap-4 text-sm text-slate-600">
          <div><strong className="text-slate-900">{t("Customer")}:</strong> {request.customerName}</div>
          <div><strong className="text-slate-900">{t("Email")}:</strong> {request.customerEmail}</div>
          <div><strong className="text-slate-900">Currency:</strong> {currencyStr}</div>
          <div><strong className="text-slate-900">{t("Date")}:</strong> {new Date(request.createdAt).toLocaleString()}</div>
          <div><strong className="text-slate-900">{t("Status")}:</strong> {request.status === 0 ? t("StatusPending") : request.status === 1 ? t("StatusSent") : t("StatusCancelled")}</div>
        </div>
      </div>

      {isPending ? (
        <div className="space-y-6">
          <div className="bg-white p-6 rounded-xl shadow border border-slate-200">
            <h2 className="text-lg font-semibold text-slate-900 mb-4">{t("UploadPricingExcel")}</h2>
            <div 
              className="border-2 border-dashed border-slate-300 rounded-xl p-12 text-center cursor-pointer hover:bg-slate-50 transition-colors"
              onClick={() => fileInputRef.current?.click()}
            >
              <input 
                type="file" 
                ref={fileInputRef} 
                onChange={handleFileUpload} 
                accept=".xlsx" 
                className="hidden" 
              />
              <div className="text-slate-500">{t("DragAndDropExcel")}</div>
            </div>
          </div>

          {parsedItems.length > 0 && (
            <div className="bg-white rounded-xl shadow border border-slate-200 overflow-hidden">
              <div className="p-4 border-b border-slate-200 bg-slate-50">
                <h3 className="font-semibold text-slate-900">{t("QuotationEditor")}</h3>
              </div>
              <div className="overflow-x-auto">
                <table className="min-w-full divide-y divide-slate-200">
                  <thead className="bg-slate-50">
                    <tr>
                      <th className="px-4 py-3 text-left text-xs font-medium text-slate-500 uppercase">{t("ProductName")}</th>
                      <th className="px-4 py-3 text-left text-xs font-medium text-slate-500 uppercase">{t("Quantity")}</th>
                      <th className="px-4 py-3 text-left text-xs font-medium text-slate-500 uppercase">{t("LastPrice")}</th>
                      <th className="px-4 py-3 text-left text-xs font-medium text-slate-500 uppercase">{t("UnitPrice")}</th>
                      <th className="px-4 py-3 text-left text-xs font-medium text-slate-500 uppercase">Discount</th>
                      <th className="px-4 py-3 text-left text-xs font-medium text-slate-500 uppercase">{t("LineTotal")}</th>
                    </tr>
                  </thead>
                  <tbody className="bg-white divide-y divide-slate-200">
                    {parsedItems.map((item) => {
                      const pricing = quotationItems[item.productId] || { unitPrice: 0, lineTotal: 0 };

                      return (
                        <tr key={item.productId}>
                          <td className="px-4 py-3 whitespace-nowrap text-sm text-slate-900 font-medium">
                            <a href={`/${locale}/admin/products/${item.productId}/history`} target="_blank" rel="noopener noreferrer" className="text-indigo-600 hover:underline">
                              {item.productName}
                            </a>
                          </td>
                          <td className="px-4 py-3 whitespace-nowrap text-sm text-slate-500">{item.quantity}</td>
                          <td className="px-4 py-3 whitespace-nowrap text-sm text-slate-500">
                            {item.hasPreviousPrice ? (
                              <div>
                                <div>{formatPrice(item.lastRequestPrice!)}</div>
                                <div className="text-xs text-slate-400">{new Date(item.lastRequestDate!).toLocaleDateString()}</div>
                              </div>
                            ) : (
                              <span className="text-amber-600 italic text-xs">{t("NoPreviousPrice")}</span>
                            )}
                          </td>
                          <td className="px-4 py-3 whitespace-nowrap text-sm">
                            <input 
                              type="number" 
                              className="w-24 border border-slate-300 rounded px-2 py-1 text-sm focus:ring-indigo-500 focus:border-indigo-500" 
                              value={pricing.unitPrice || ''}
                              onChange={(e) => handlePriceOrDiscountChange(item.productId, item.quantity, e.target.value, 'price')}
                            />
                          </td>
                          <td className="px-4 py-3 whitespace-nowrap text-sm">
                            <input 
                              type="number" 
                              className="w-24 border border-slate-300 rounded px-2 py-1 text-sm focus:ring-indigo-500 focus:border-indigo-500" 
                              value={pricing.discount || ''}
                              onChange={(e) => handlePriceOrDiscountChange(item.productId, item.quantity, e.target.value, 'discount')}
                            />
                          </td>

                          <td className="px-4 py-3 whitespace-nowrap text-sm font-medium text-slate-900">
                            {formatPrice(pricing.lineTotal)}
                          </td>
                        </tr>
                      );
                    })}
                  </tbody>
                </table>
              </div>
              <div className="p-6 bg-slate-50 border-t border-slate-200 flex justify-between items-center">
                <div className="text-xl font-bold text-slate-900">
                  {t("GrandTotal")}: {formatPrice(grandTotal)} {currencyStr}
                </div>
                <button 
                  onClick={handleSubmitQuotation}
                  disabled={isSubmitting}
                  className="bg-indigo-600 hover:bg-indigo-700 text-white px-6 py-3 rounded-lg shadow font-medium transition-colors disabled:opacity-50 cursor-pointer disabled:cursor-not-allowed"
                >
                  {isSubmitting ? tCommon("Loading") : t("SubmitQuotation")}
                </button>
              </div>
            </div>
          )}
        </div>
      ) : (
        <div className="bg-white rounded-xl shadow border border-slate-200 overflow-hidden">
          <div className="p-4 border-b border-slate-200 bg-slate-50">
            <h3 className="font-semibold text-slate-900">{t("FinalQuotation")}</h3>
          </div>
          <table className="min-w-full divide-y divide-slate-200">
            <thead className="bg-slate-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">{t("ProductName")}</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">{t("Quantity")}</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">{t("UnitPrice")}</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">Discount</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-slate-500 uppercase">{t("LineTotal")}</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-slate-200">
              {request.items.map((item) => (
                <tr key={item.id}>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-slate-900">{item.productName}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-slate-500">{item.quantity}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-slate-500">{formatPrice(item.unitPrice)}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-slate-500">{formatPrice(item.discount)}</td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-slate-900">{formatPrice(item.lineTotal)}</td>
                </tr>
              ))}
            </tbody>
          </table>
          <div className="p-6 bg-slate-50 border-t border-slate-200 text-right">
            <div className="text-2xl font-bold text-slate-900">
              {t("GrandTotal")}: {formatPrice(request.totalAmount)} {currencyStr}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
