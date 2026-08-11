"use client";

import { useEffect, useState, FormEvent } from "react";
import { useRouter } from "next/navigation";
import ProtectedRoute from "@/components/ProtectedRoute";
import apiClient from "@/lib/apiClient";

interface Subject {
  id: number;
  name: string;
  className: string;
}

export default function CreateAssignment() {
  const [subjects, setSubjects] = useState<Subject[]>([]);
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [subjectId, setSubjectId] = useState("");
  const [deadline, setDeadline] = useState("");
  const [maxMarks, setMaxMarks] = useState(100);
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const router = useRouter();

  useEffect(() => {
    apiClient.get<Subject[]>("/subjects/my").then((res) => setSubjects(res.data));
  }, []);

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      await apiClient.post("/assignments", {
        title,
        description,
        subjectId: Number(subjectId),
        deadline: new Date(deadline).toISOString(),
        maxMarks,
      });
      router.push("/teacher/assignments");
    } catch (err: any) {
      setError(err.response?.data ?? "Failed to create assignment.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <ProtectedRoute allowedRoles={["Teacher"]}>
      <div className="min-h-screen bg-gray-50 p-8">
        <h1 className="text-2xl font-bold text-gray-900 mb-6">Create Assignment</h1>

        <form onSubmit={handleSubmit} className="bg-white p-6 rounded-lg shadow max-w-lg space-y-4">
          {error && <div className="bg-red-100 text-red-700 p-2 rounded text-sm">{error}</div>}

          <div>
            <label className="block text-sm font-medium mb-1 text-gray-900">Title</label>
            <input
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              required
              className="w-full border border-gray-300 rounded px-3 py-2 text-gray-900"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 text-gray-900">Description</label>
            <textarea
              value={description}
              onChange={(e) => setDescription(e.target.value)}
              required
              rows={3}
              className="w-full border border-gray-300 rounded px-3 py-2 text-gray-900"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 text-gray-900">Subject</label>
            <select
              value={subjectId}
              onChange={(e) => setSubjectId(e.target.value)}
              required
              className="w-full border border-gray-300 rounded px-3 py-2 text-gray-900"
            >
              <option value="">Select a subject</option>
              {subjects.map((s) => (
                <option key={s.id} value={s.id}>
                  {s.name} ({s.className})
                </option>
              ))}
            </select>
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 text-gray-900">Deadline</label>
            <input
              type="datetime-local"
              value={deadline}
              onChange={(e) => setDeadline(e.target.value)}
              required
              className="w-full border border-gray-300 rounded px-3 py-2 text-gray-900"
            />
          </div>

          <div>
            <label className="block text-sm font-medium mb-1 text-gray-900">Max Marks</label>
            <input
              type="number"
              value={maxMarks}
              onChange={(e) => setMaxMarks(Number(e.target.value))}
              required
              min={1}
              className="w-full border border-gray-300 rounded px-3 py-2 text-gray-900"
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full bg-blue-600 text-white py-2 rounded hover:bg-blue-700 disabled:opacity-50"
          >
            {loading ? "Creating..." : "Create Assignment"}
          </button>
        </form>
      </div>
    </ProtectedRoute>
  );
}