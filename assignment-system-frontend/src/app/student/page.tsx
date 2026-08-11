"use client";

import ProtectedRoute from "@/components/ProtectedRoute";
import { useAuth } from "@/context/AuthContext";

export default function StudentDashboard() {
  const { user, logout } = useAuth();

  return (
    <ProtectedRoute allowedRoles={["Student"]}>
      <div className="min-h-screen bg-gray-50 p-8">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold text-gray-900">Student Dashboard</h1>
          <button
            onClick={logout}
            className="bg-red-600 text-white px-4 py-2 rounded hover:bg-red-700"
          >
            Logout
          </button>
        </div>
        <p className="text-gray-700">Welcome, {user?.fullName}.</p>
      </div>
    </ProtectedRoute>
  );
}