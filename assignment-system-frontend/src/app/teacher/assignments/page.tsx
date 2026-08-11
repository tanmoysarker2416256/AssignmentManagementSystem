"use client";

import { useEffect, useState } from "react";
import ProtectedRoute from "@/components/ProtectedRoute";
import apiClient from "@/lib/apiClient";
import Link from "next/link";

interface Assignment {
  id: number;
  title: string;
  subjectName: string;
  deadline: string;
  maxMarks: number;
  status: string;
  createdAt: string;
}

export default function TeacherAssignments() {
  const [assignments, setAssignments] = useState<Assignment[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    fetchAssignments();
  }, []);

  async function fetchAssignments() {
    setLoading(true);
    try {
      const response = await apiClient.get<Assignment[]>("/assignments/my");
      setAssignments(response.data);
    } catch {
      setError("Failed to load assignments.");
    } finally {
      setLoading(false);
    }
  }

  async function handlePublish(id: number) {
    try {
      await apiClient.patch(`/assignments/${id}/publish`);
      fetchAssignments(); // refresh the list to show updated status
    } catch {
      setError("Failed to publish assignment.");
    }
  }

  return (
    <ProtectedRoute allowedRoles={["Teacher"]}>
      <div className="min-h-screen bg-gray-50 p-8">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold text-gray-900">My Assignments</h1>
          <Link
            href="/teacher/assignments/create"
            className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
          >
            + Create Assignment
          </Link>
        </div>

        {error && (
          <div className="bg-red-100 text-red-700 p-2 rounded mb-4">{error}</div>
        )}

        {loading ? (
          <p className="text-gray-700">Loading...</p>
        ) : assignments.length === 0 ? (
          <p className="text-gray-700">No assignments yet.</p>
        ) : (
          <div className="bg-white rounded-lg shadow overflow-hidden">
            <table className="w-full text-left">
              <thead className="bg-gray-100 text-gray-700 text-sm">
                <tr>
                  <th className="p-3">Title</th>
                  <th className="p-3">Subject</th>
                  <th className="p-3">Deadline</th>
                  <th className="p-3">Max Marks</th>
                  <th className="p-3">Status</th>
                  <th className="p-3">Action</th>
                </tr>
              </thead>
              <tbody>
                {assignments.map((a) => (
                  <tr key={a.id} className="border-t text-gray-900">
                    <td className="p-3">{a.title}</td>
                    <td className="p-3">{a.subjectName}</td>
                    <td className="p-3">{new Date(a.deadline).toLocaleString()}</td>
                    <td className="p-3">{a.maxMarks}</td>
                    <td className="p-3">
                      <span
                        className={`px-2 py-1 rounded text-xs font-medium ${
                          a.status === "Published"
                            ? "bg-green-100 text-green-700"
                            : "bg-yellow-100 text-yellow-700"
                        }`}
                      >
                        {a.status}
                      </span>
                    </td>
                    <td className="p-3">
                      {a.status === "Draft" && (
                        <button
                          onClick={() => handlePublish(a.id)}
                          className="text-blue-600 hover:underline text-sm"
                        >
                          Publish
                        </button>
                      )}
                      <Link
                        href={`/teacher/assignments/${a.id}/submissions`}
                        className="text-gray-600 hover:underline text-sm ml-3"
                      >
                        Submissions
                      </Link>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </ProtectedRoute>
  );
}