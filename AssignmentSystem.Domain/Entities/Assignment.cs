using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Domain.Entities;

public enum AssignmentStatus { Draft, Published }

public class Assignment
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Description { get; set; } = default!;
    public DateTime Deadline { get; set; }
    public int MaxMarks { get; set; }
    public AssignmentStatus Status { get; set; } = AssignmentStatus.Draft;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = default!;

    public string TeacherId { get; set; } = default!;
    public ApplicationUser Teacher { get; set; } = default!;

    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
