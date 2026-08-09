using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Domain.Entities;

public enum SubmissionStatus { Submitted, Late, Graded }

public class Submission
{
    public int Id { get; set; }
    public string Content { get; set; } = default!;
    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    public SubmissionStatus Status { get; set; } = SubmissionStatus.Submitted;
    public int? Marks { get; set; }
    public string? Feedback { get; set; }
    public DateTime? GradedAt { get; set; }

    public int AssignmentId { get; set; }
    public Assignment Assignment { get; set; } = default!;

    public string StudentId { get; set; } = default!;
    public ApplicationUser Student { get; set; } = default!;
}
