"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuth } from "@/context/AuthContext";
import { Spinner } from "@/components/ui/Spinner";

export default function Home() {
  const { role, isLoading } = useAuth();
  const router = useRouter();

  useEffect(() => {
    if (!isLoading) {
      if (role === "Admin") {
        router.push("/admin");
      } else {
        router.push("/products");
      }
    }
  }, [role, isLoading, router]);

  return <div className="p-8 flex justify-center items-center h-screen"><Spinner size="lg" /></div>;
}
