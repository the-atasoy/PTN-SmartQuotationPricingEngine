

export interface ParsedExcelResultDto {
  requestNo: string;
  productId: string;
  productName: string;
  quantity: number;
  lastRequestPrice?: number;
  lastRequestDate?: string;
  hasPreviousPrice: boolean;
}

export const excelApi = {
  parseExcel: async (file: File, requestId: string, token: string | null = null) => {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('requestId', requestId);
    
    const res = await fetch(`${process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5100'}/api/v1/excel/parse`, {
      method: 'POST',
      headers: {
        ...(token ? { 'Authorization': `Bearer ${token}` } : {})
      },
      body: formData
    });

    if (!res.ok) {
      const err = await res.json().catch(() => null);
      if (err && err.errors && err.errors.length > 0) {
        throw new Error(err.errors[0]);
      }
      throw new Error(`Failed to parse Excel: ${res.statusText}`);
    }

    return res.json() as Promise<{ data: ParsedExcelResultDto[], isSuccessful: boolean, errors?: string[] }>;
  }
};
