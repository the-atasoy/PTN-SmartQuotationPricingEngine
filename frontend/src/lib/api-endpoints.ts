export const API_ENDPOINTS = {
  AUTH: {
    LOGIN: "/api/v1/auth/login",
    LOGOUT: "/api/v1/auth/logout",
    REFRESH: "/api/v1/auth/refresh",
  },
  PRODUCTS: {
    BASE: "/api/v1/products",
  },
  REQUESTS: {
    BASE: "/api/v1/requests",
  },
  EXCEL: {
    BASE: "/api/v1/excel",
  }
};

export const getApiUrl = (endpoint: string) => {
  const baseUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5100";
  return `${baseUrl}${endpoint}`;
};
