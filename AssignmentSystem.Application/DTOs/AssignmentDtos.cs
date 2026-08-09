using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.DTOs;

public record CreateAssignmentDto(
    string Title,
    string Description,
    int SubjectId,
    DateTime Deadline,
    int MaxMarks
);

public record AssignmentDto(
    int Id,
    string Title,
    string Description,
    DateTime Deadline,
    int MaxMarks,
    string Status,
    DateTime CreatedAt,
    int SubjectId,
    string SubjectName,
    string TeacherId,
    string TeacherName
);