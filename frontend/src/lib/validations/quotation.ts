import * as z from "zod";

export const quotationItemSchema = z.object({
  productId: z.string().uuid(),
  unitPrice: z.number().positive("Unit price must be greater than zero"),
  discount: z.number().min(0).max(100, "Discount cannot exceed 100%").default(0),
});

export const sendQuotationSchema = z.object({
  requestId: z.string().uuid(),
  items: z.array(quotationItemSchema).min(1, "At least one item is required"),
});

export type QuotationItemInput = z.infer<typeof quotationItemSchema>;
export type SendQuotationInput = z.infer<typeof sendQuotationSchema>;
