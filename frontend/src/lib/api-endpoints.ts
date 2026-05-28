export const API_ENDPOINTS = {
  AUTH: {
    LOGIN: "/api/v1/auth/login",
    LOGOUT: "/api/v1/auth/logout",
    REFRESH: "/api/v1/auth/refresh",
  },
  PRODUCTS: {
    GET_ALL: "/api/v1/products",
  },
  REQUESTS: {
    CREATE: "/api/v1/requests",
  }
};

export const getApiUrl = (endpoint: string) => {
  const baseUrl = process.env.NEXT_PUBLIC_API_URL || "http://localhost:5100";
  return `${baseUrl}${endpoint}`;
};
