using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Domain.Entities;

public class Subject
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public int ClassId { get; set; }
    public Class Class { get; set; } = default!;

    public ICollection<TeacherSubject> TeacherSubjects { get; set; } = new List<TeacherSubject>();
    public ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
}
