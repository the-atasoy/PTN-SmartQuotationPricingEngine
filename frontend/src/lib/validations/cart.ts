import * as z from "zod";

export const requestQuoteSchema = z.object({
  email: z.string().min(1, "Email is required").email("Invalid email address"),
});

export type RequestQuoteFormData = z.infer<typeof requestQuoteSchema>;
