import * as z from "zod";

export const requestQuoteSchema = z.object({
  email: z.string().min(1, "Email is required").email("Invalid email address"),
  currency: z.enum(["TRY", "USD", "EUR"], { message: "Currency is required" }),
});

export type RequestQuoteFormData = z.infer<typeof requestQuoteSchema>;
