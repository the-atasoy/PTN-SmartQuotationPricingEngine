import { getApiUrl, API_ENDPOINTS } from '../api-endpoints';
import { getHeaders } from './api-client';
import { PaginatedResult, ApiResponse, Product, PriceHistoryDto } from '../types/api';

export const productsApi = {
  getAll: async (page: number = 0, pageSize: number = 10, token: string | null = null) => {
    const res = await fetch(`${getApiUrl(API_ENDPOINTS.PRODUCTS.BASE)}?page=${page}&pageSize=${pageSize}`, {
      method: 'GET',
      headers: getHeaders(token)
    });
    if (!res.ok) throw new Error('Failed to fetch products');
    return res.json() as Promise<ApiResponse<PaginatedResult<Product>>>;
  },

  getPriceHistory: async (id: string, page: number = 0, pageSize: number = 10, token: string | null = null) => {
    const res = await fetch(`${getApiUrl(API_ENDPOINTS.PRODUCTS.BASE)}/${id}/price-history?page=${page}&pageSize=${pageSize}`, {
      method: 'GET',
      headers: getHeaders(token)
    });
    if (!res.ok) throw new Error('Failed to fetch price history');
    return res.json() as Promise<ApiResponse<PaginatedResult<PriceHistoryDto>>>;
  }
};
