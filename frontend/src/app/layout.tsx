import type { Metadata } from "next";
import "./globals.css";
import { defaultLocale } from "@/i18n";

export const metadata: Metadata = {
  title: "Smart Quotation",
  description: "Smart Quotation Pricing Engine",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang={defaultLocale} className="h-full antialiased">
      <body className="min-h-full flex flex-col">{children}</body>
    </html>
  );
}
