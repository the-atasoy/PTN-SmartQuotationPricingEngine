import { getApiUrl, API_ENDPOINTS } from '../api-endpoints';
import { getHeaders } from './api-client';
import { PaginatedResult, ApiResponse } from '../types/api';

export interface RequestListItemDto {
  id: string;
  requestNo: string;
  customerName: string;
  customerEmail: string;
  totalAmount: number;
  currency: number;
  status: number;
  createdAt: string;
  lastModified?: string;
}

export interface RequestItemDetailDto {
  id: string;
  productId: string;
  productName: string;
  quantity: number;
  unitPrice: number;
  discount: number;
  lineTotal: number;
  lastRequestPrice?: number;
  lastRequestCurrency?: number;
  lastRequestDate?: string;
}

export interface RequestDetailDto {
  id: string;
  requestNo: string;
  customerId: string;
  customerName: string;
  customerEmail: string;
  totalAmount: number;
  currency: number;
  status: number;
  createdAt: string;
  lastModified?: string;
  items: RequestItemDetailDto[];
}

export interface SendQuotationInput {
  requestId: string;
  items: {
    productId: string;
    unitPrice: number;
    discount: number;
  }[];
}

export const requestsApi = {

  getAll: async (page: number = 0, pageSize: number = 10, token: string | null = null) => {
    const res = await fetch(`${getApiUrl(API_ENDPOINTS.REQUESTS.BASE)}?page=${page}&pageSize=${pageSize}`, {
      method: 'GET',
      headers: getHeaders(token)
    });
    if (!res.ok) {
      const errText = await res.text();
      console.error(`Failed to fetch requests. Status: ${res.status}, Body: ${errText}`);
      throw new Error(`Failed to fetch requests: ${res.status} ${errText}`);
    }
    return res.json() as Promise<ApiResponse<PaginatedResult<RequestListItemDto>>>;
  },

  getById: async (id: string, token: string | null = null) => {
    const res = await fetch(`${getApiUrl(API_ENDPOINTS.REQUESTS.BASE)}/${id}`, {
      method: 'GET',
      headers: getHeaders(token)
    });
    if (!res.ok) throw new Error('Failed to fetch request details');
    return res.json() as Promise<ApiResponse<RequestDetailDto>>;
  },

  sendQuotation: async (id: string, data: SendQuotationInput, token: string | null = null) => {
    const res = await fetch(`${getApiUrl(API_ENDPOINTS.REQUESTS.BASE)}/${id}/send`, {
      method: 'PUT',
      headers: getHeaders(token),
      body: JSON.stringify(data)
    });
    if (!res.ok) {
        const errorData = await res.json().catch(() => ({}));
        throw { response: { data: errorData } };
    }
    return res.json();
  },

  downloadExcel: async (id: string, token: string | null = null) => {
    const res = await fetch(`${getApiUrl(API_ENDPOINTS.REQUESTS.BASE)}/${id}/excel`, {
      method: 'GET',
      headers: getHeaders(token)
    });
    if (!res.ok) {
        const errorText = await res.text();
        console.error("Excel download failed:", res.status, errorText);
        throw new Error('Failed to download excel');
    }
    return res.blob();
  },
};
