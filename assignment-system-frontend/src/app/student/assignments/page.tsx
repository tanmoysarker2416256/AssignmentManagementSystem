"use client";

import { useEffect, useState } from "react";
import ProtectedRoute from "@/components/ProtectedRoute";
import apiClient from "@/lib/apiClient";
import Link from "next/link";

interface Assignment {
  id: number;
  title: string;
  subjectName: string;
  teacherName: string;
  deadline: string;
  maxMarks: number;
}

export default function StudentAssignments() {
  const [assignments, setAssignments] = useState<Assignment[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    apiClient
      .get<Assignment[]>("/assignments/available")
      .then((res) => setAssignments(res.data))
      .catch(() => setError("Failed to load assignments."))
      .finally(() => setLoading(false));
  }, []);

  return (
    <ProtectedRoute allowedRoles={["Student"]}>
      <div className="min-h-screen bg-gray-50 p-8">
        <h1 className="text-2xl font-bold text-gray-900 mb-6">Available Assignments</h1>

        {error && <div className="bg-red-100 text-red-700 p-2 rounded mb-4">{error}</div>}

        {loading ? (
          <p className="text-gray-700">Loading...</p>
        ) : assignments.length === 0 ? (
          <p className="text-gray-700">No assignments available right now.</p>
        ) : (
          <div className="space-y-3">
            {assignments.map((a) => {
              const isPastDeadline = new Date(a.deadline) < new Date();
              return (
                <Link
                  key={a.id}
                  href={`/student/assignments/${a.id}`}
                  className="block bg-white p-4 rounded-lg shadow hover:shadow-md transition"
                >
                  <div className="flex justify-between items-start">
                    <div>
                      <p className="font-semibold text-gray-900">{a.title}</p>
                      <p className="text-sm text-gray-600">
                        {a.subjectName} · {a.teacherName}
                      </p>
                    </div>
                    <span
                      className={`px-2 py-1 rounded text-xs font-medium ${
                        isPastDeadline
                          ? "bg-red-100 text-red-700"
                          : "bg-blue-100 text-blue-700"
                      }`}
                    >
                      {isPastDeadline ? "Deadline passed" : "Due " + new Date(a.deadline).toLocaleDateString()}
                    </span>
                  </div>
                </Link>
              );
            })}
          </div>
        )}
      </div>
    </ProtectedRoute>
  );
}