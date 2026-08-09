using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Domain.Entities;

public class TeacherSubject
{
    public int Id { get; set; }
    public string TeacherId { get; set; } = default!;
    public ApplicationUser Teacher { get; set; } = default!;

    public int SubjectId { get; set; }
    public Subject Subject { get; set; } = default!;
}
