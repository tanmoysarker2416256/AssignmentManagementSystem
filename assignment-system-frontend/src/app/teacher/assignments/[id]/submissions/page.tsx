"use client";

import { useEffect, useState } from "react";
import { useParams } from "next/navigation";
import ProtectedRoute from "@/components/ProtectedRoute";
import apiClient from "@/lib/apiClient";

interface Submission {
  id: number;
  studentName: string;
  content: string;
  submittedAt: string;
  status: string;
  marks: number | null;
  feedback: string | null;
}

export default function AssignmentSubmissions() {
  const params = useParams();
  const assignmentId = params.id as string;

  const [submissions, setSubmissions] = useState<Submission[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [gradingId, setGradingId] = useState<number | null>(null);
  const [marks, setMarks] = useState("");
  const [feedback, setFeedback] = useState("");

  useEffect(() => {
    fetchSubmissions();
  }, [assignmentId]);

  async function fetchSubmissions() {
    setLoading(true);
    try {
      const res = await apiClient.get<Submission[]>(`/submissions/assignment/${assignmentId}`);
      setSubmissions(res.data);
    } catch {
      setError("Failed to load submissions.");
    } finally {
      setLoading(false);
    }
  }

  function startGrading(sub: Submission) {
    setGradingId(sub.id);
    setMarks(sub.marks?.toString() ?? "");
    setFeedback(sub.feedback ?? "");
  }

  async function submitGrade(submissionId: number) {
    setError("");
    try {
      await apiClient.patch(`/submissions/${submissionId}/grade`, {
        marks: Number(marks),
        feedback,
      });
      setGradingId(null);
      fetchSubmissions();
    } catch (err: any) {
      setError(err.response?.data ?? "Failed to submit grade.");
    }
  }

  return (
    <ProtectedRoute allowedRoles={["Teacher"]}>
      <div className="min-h-screen bg-gray-50 p-8">
        <h1 className="text-2xl font-bold text-gray-900 mb-6">Submissions</h1>

        {error && <div className="bg-red-100 text-red-700 p-2 rounded mb-4">{error}</div>}

        {loading ? (
          <p className="text-gray-700">Loading...</p>
        ) : submissions.length === 0 ? (
          <p className="text-gray-700">No submissions yet.</p>
        ) : (
          <div className="space-y-4">
            {submissions.map((sub) => (
              <div key={sub.id} className="bg-white p-4 rounded-lg shadow">
                <div className="flex justify-between items-start mb-2">
                  <div>
                    <p className="font-semibold text-gray-900">{sub.studentName}</p>
                    <p className="text-xs text-gray-500">
                      Submitted: {new Date(sub.submittedAt).toLocaleString()}
                    </p>
                  </div>
                  <span
                    className={`px-2 py-1 rounded text-xs font-medium ${
                      sub.status === "Graded"
                        ? "bg-green-100 text-green-700"
                        : sub.status === "Late"
                        ? "bg-red-100 text-red-700"
                        : "bg-yellow-100 text-yellow-700"
                    }`}
                  >
                    {sub.status}
                  </span>
                </div>

                <p className="text-gray-800 mb-3 whitespace-pre-wrap">{sub.content}</p>

                {gradingId === sub.id ? (
                  <div className="space-y-2 border-t pt-3">
                    <input
                      type="number"
                      placeholder="Marks"
                      value={marks}
                      onChange={(e) => setMarks(e.target.value)}
                      className="w-full border border-gray-300 rounded px-3 py-2 text-gray-900"
                    />
                    <textarea
                      placeholder="Feedback"
                      value={feedback}
                      onChange={(e) => setFeedback(e.target.value)}
                      rows={2}
                      className="w-full border border-gray-300 rounded px-3 py-2 text-gray-900"
                    />
                    <div className="flex gap-2">
                      <button
                        onClick={() => submitGrade(sub.id)}
                        className="bg-blue-600 text-white px-4 py-1.5 rounded hover:bg-blue-700 text-sm"
                      >
                        Save Grade
                      </button>
                      <button
                        onClick={() => setGradingId(null)}
                        className="bg-gray-200 text-gray-700 px-4 py-1.5 rounded hover:bg-gray-300 text-sm"
                      >
                        Cancel
                      </button>
                    </div>
                  </div>
                ) : (
                  <div className="border-t pt-3 flex justify-between items-center">
                    <div className="text-sm text-gray-700">
                      {sub.marks !== null ? (
                        <>
                          <span className="font-medium">Marks: {sub.marks}</span>
                          {sub.feedback && <span className="ml-3">Feedback: {sub.feedback}</span>}
                        </>
                      ) : (
                        <span className="text-gray-500">Not graded yet</span>
                      )}
                    </div>
                    <button
                      onClick={() => startGrading(sub)}
                      className="text-blue-600 hover:underline text-sm"
                    >
                      {sub.marks !== null ? "Edit Grade" : "Grade"}
                    </button>
                  </div>
                )}
              </div>
            ))}
          </div>
        )}
      </div>
    </ProtectedRoute>
  );
}