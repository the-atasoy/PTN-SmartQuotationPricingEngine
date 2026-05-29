import * as z from "zod";

export const requestQuoteSchema = z.object({
  companyName: z.string().min(2, { message: "Company name must be at least 2 characters" }),
});

export type RequestQuoteFormData = z.infer<typeof requestQuoteSchema>;
