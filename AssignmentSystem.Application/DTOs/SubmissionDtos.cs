using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.DTOs;

public record SubmitDto(int AssignmentId, string Content);

public record GradeDto(int Marks, string Feedback);

public record SubmissionDto(
    int Id,
    int AssignmentId,
    string AssignmentTitle,
    string StudentId,
    string StudentName,
    string Content,
    DateTime SubmittedAt,
    string Status,
    int? Marks,
    string? Feedback,
    DateTime? GradedAt
);
