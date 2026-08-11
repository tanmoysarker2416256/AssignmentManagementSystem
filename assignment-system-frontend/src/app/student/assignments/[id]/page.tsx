"use client";

import { useEffect, useState, FormEvent } from "react";
import { useParams } from "next/navigation";
import ProtectedRoute from "@/components/ProtectedRoute";
import apiClient from "@/lib/apiClient";

interface Assignment {
  id: number;
  title: string;
  description: string;
  subjectName: string;
  teacherName: string;
  deadline: string;
  maxMarks: number;
}

interface MySubmission {
  id: number;
  assignmentId: number;
  content: string;
  status: string;
  marks: number | null;
  feedback: string | null;
  submittedAt: string;
}

export default function AssignmentDetail() {
  const params = useParams();
  const assignmentId = Number(params.id);

  const [assignment, setAssignment] = useState<Assignment | null>(null);
  const [existingSubmission, setExistingSubmission] = useState<MySubmission | null>(null);
  const [content, setContent] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  useEffect(() => {
    loadData();
  }, [assignmentId]);

  async function loadData() {
    setLoading(true);
    try {
      const [assignmentRes, mySubsRes] = await Promise.all([
        apiClient.get<Assignment>(`/assignments/${assignmentId}`),
        apiClient.get<MySubmission[]>("/submissions/my"),
      ]);
      setAssignment(assignmentRes.data);

      const existing = mySubsRes.data.find((s) => s.assignmentId === assignmentId);
      if (existing) {
        setExistingSubmission(existing);
        setContent(existing.content);
      }
    } catch {
      setError("Failed to load assignment.");
    } finally {
      setLoading(false);
    }
  }

  async function handleSubmit(e: FormEvent) {
    e.preventDefault();
    setError("");
    setSuccess("");
    try {
      await apiClient.post("/submissions", { assignmentId, content });
      setSuccess("Submitted successfully!");
      loadData(); // refresh to show updated status
    } catch (err: any) {
      setError(err.response?.data ?? "Failed to submit.");
    }
  }

  if (loading) return <p className="p-8 text-gray-700">Loading...</p>;
  if (!assignment) return <p className="p-8 text-gray-700">Assignment not found.</p>;

  const isPastDeadline = new Date(assignment.deadline) < new Date();
  const isGraded = existingSubmission?.status === "Graded";

  return (
    <ProtectedRoute allowedRoles={["Student"]}>
      <div className="min-h-screen bg-gray-50 p-8">
        <div className="bg-white p-6 rounded-lg shadow max-w-2xl mx-auto">
          <h1 className="text-2xl font-bold text-gray-900 mb-1">{assignment.title}</h1>
          <p className="text-sm text-gray-600 mb-4">
            {assignment.subjectName} · {assignment.teacherName} · Max Marks: {assignment.maxMarks}
          </p>
          <p className="text-gray-800 mb-4 whitespace-pre-wrap">{assignment.description}</p>
          <p className="text-sm text-gray-600 mb-6">
            Deadline: {new Date(assignment.deadline).toLocaleString()}
            {isPastDeadline && <span className="text-red-600 ml-2">(Passed)</span>}
          </p>

          {existingSubmission?.status && (
            <div className="mb-4 p-3 bg-gray-100 rounded">
              <p className="text-sm font-medium text-gray-900">
                Status: {existingSubmission.status}
              </p>
              {existingSubmission.marks !== null && (
                <p className="text-sm text-gray-800 mt-1">
                  Marks: {existingSubmission.marks} / {assignment.maxMarks}
                </p>
              )}
              {existingSubmission.feedback && (
                <p className="text-sm text-gray-800 mt-1">Feedback: {existingSubmission.feedback}</p>
              )}
            </div>
          )}

          {error && <div className="bg-red-100 text-red-700 p-2 rounded mb-4 text-sm">{error}</div>}
          {success && <div className="bg-green-100 text-green-700 p-2 rounded mb-4 text-sm">{success}</div>}

          {isGraded ? (
            <p className="text-sm text-gray-500">This submission has been graded and can no longer be edited.</p>
          ) : isPastDeadline && !existingSubmission ? (
            <p className="text-sm text-gray-500">The deadline has passed. You did not submit an answer.</p>
          ) : isPastDeadline && existingSubmission ? (
            <p className="text-sm text-gray-500">The deadline has passed. Your submission can no longer be updated.</p>
          ) : (
            <form onSubmit={handleSubmit} className="space-y-3">
              <textarea
                value={content}
                onChange={(e) => setContent(e.target.value)}
                required
                rows={6}
                placeholder="Write your answer here..."
                className="w-full border border-gray-300 rounded px-3 py-2 text-gray-900"
              />
              <button
                type="submit"
                className="bg-blue-600 text-white px-4 py-2 rounded hover:bg-blue-700"
              >
                {existingSubmission ? "Update Submission" : "Submit"}
              </button>
            </form>
          )}
        </div>
      </div>
    </ProtectedRoute>
  );
}