using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Domain.Entities;

public class Class
{
    public int Id { get; set; }
    public string Name { get; set; } = default!; 

    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    public ICollection<ApplicationUser> Students { get; set; } = new List<ApplicationUser>();
}
