import * as z from "zod";

export const requestQuoteSchema = z.object({});

export type RequestQuoteFormData = z.infer<typeof requestQuoteSchema>;
