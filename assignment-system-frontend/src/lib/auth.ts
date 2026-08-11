import { jwtDecode } from "jwt-decode";

export interface DecodedToken {
  sub: string;
  email: string;
  fullName: string;
  "http://schemas.microsoft.com/ws/2008/06/identity/claims/role": string;
  exp: number;
}

export interface User {
  id: string;
  email: string;
  fullName: string;
  role: string;
}

export function decodeToken(token: string): User | null {
  try {
    const decoded = jwtDecode<DecodedToken>(token);
    return {
      id: decoded.sub,
      email: decoded.email,
      fullName: decoded.fullName,
      role: decoded["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"],
    };
  } catch {
    return null;
  }
}