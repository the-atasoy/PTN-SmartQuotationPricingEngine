import * as z from "zod";

export const requestQuoteSchema = z.object({
  currency: z.enum(["TRY", "USD", "EUR"], { message: "Currency is required" }),
});

export type RequestQuoteFormData = z.infer<typeof requestQuoteSchema>;
