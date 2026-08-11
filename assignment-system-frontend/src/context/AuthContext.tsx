"use client";

import { createContext, useContext, useEffect, useState, ReactNode } from "react";
import { useRouter } from "next/navigation";
import apiClient from "@/lib/apiClient";
import { decodeToken, User } from "@/lib/auth";

interface AuthContextType {
  user: User | null;
  loading: boolean;
  login: (email: string, password: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const router = useRouter();

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (token) {
      const decoded = decodeToken(token);
      setUser(decoded);
    }
    setLoading(false);
  }, []);

  async function login(email: string, password: string) {
    const response = await apiClient.post("/auth/login", { email, password });
    const { token } = response.data;
    localStorage.setItem("token", token);
    const decoded = decodeToken(token);
    setUser(decoded);

    if (decoded?.role === "Admin") router.push("/admin");
    else if (decoded?.role === "Teacher") router.push("/teacher");
    else if (decoded?.role === "Student") router.push("/student");
  }

  function logout() {
    localStorage.removeItem("token");
    setUser(null);
    router.push("/login");
  }

  return (
    <AuthContext.Provider value={{ user, loading, login, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error("useAuth must be used within AuthProvider");
  return context;
}