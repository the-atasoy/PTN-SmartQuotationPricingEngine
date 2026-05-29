export interface PaginatedResult<T> {
  items: T[];
  totalCount: number;
  totalPages: number;
  page: number;
  pageSize: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

export interface ApiResponse<T = unknown> {
  data: T;
  isSuccessful: boolean;
  statusCode: number;
  errors?: string[];
}

export interface Product {
  id: string;
  name: string;
  basePrice: number;
  basePriceCurrency: number;
  lastRequestPrice: number | null;
  lastRequestCurrency: number | null;
  lastRequestDate: string | null;
}

export interface PriceHistoryDto {
  id: string;
  requestId: string;
  requestNo: string;
  customerName: string;
  price: number;
  currency: number;
  createdAt: string;
}
