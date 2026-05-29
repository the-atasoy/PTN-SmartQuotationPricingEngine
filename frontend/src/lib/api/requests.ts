

export interface CreateRequestInput {
  customerId: string;
  currency: number;
  items: {
    productId: string;
    quantity: number;
  }[];
}

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
  lineTotal: number;
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
  }[];
}

import { getApiUrl, API_ENDPOINTS } from '../api-endpoints';

const getHeaders = (token: string | null) => {
  return {
    'Content-Type': 'application/json',
    ...(token ? { 'Authorization': `Bearer ${token}` } : {})
  };
};

export const requestsApi = {
  createRequest: async (data: CreateRequestInput, token: string | null) => {
    const res = await fetch(getApiUrl(API_ENDPOINTS.REQUESTS.CREATE), {
      method: 'POST',
      headers: getHeaders(token),
      body: JSON.stringify(data)
    });
    if (!res.ok) throw new Error('Failed to create request');
    return res.json();
  },

  getAll: async (page: number = 0, pageSize: number = 10, token: string | null = null) => {
    const res = await fetch(`${getApiUrl(API_ENDPOINTS.REQUESTS.CREATE)}?page=${page}&pageSize=${pageSize}`, {
      method: 'GET',
      headers: getHeaders(token)
    });
    if (!res.ok) {
      const errText = await res.text();
      console.error(`Failed to fetch requests. Status: ${res.status}, Body: ${errText}`);
      throw new Error(`Failed to fetch requests: ${res.status} ${errText}`);
    }
    return res.json() as Promise<{ data: { items: RequestListItemDto[], totalCount: number, totalPages: number, page: number, pageSize: number, hasNextPage: boolean, hasPreviousPage: boolean } }>;
  },

  getById: async (id: string, token: string | null = null) => {
    const res = await fetch(`${getApiUrl(API_ENDPOINTS.REQUESTS.CREATE)}/${id}`, {
      method: 'GET',
      headers: getHeaders(token)
    });
    if (!res.ok) throw new Error('Failed to fetch request details');
    return res.json() as Promise<{ data: RequestDetailDto }>;
  },

  sendQuotation: async (id: string, data: SendQuotationInput, token: string | null = null) => {
    const res = await fetch(`${getApiUrl(API_ENDPOINTS.REQUESTS.CREATE)}/${id}/send`, {
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

  downloadExcelUrl: (id: string) =>
    `${process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5100'}/api/v1/requests/${id}/excel`,
};
