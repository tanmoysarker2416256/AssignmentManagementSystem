using AssignmentSystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<Class> Classes { get; }
    IRepository<Subject> Subjects { get; }
    IRepository<TeacherSubject> TeacherSubjects { get; }
    IRepository<Assignment> Assignments { get; }
    IRepository<Submission> Submissions { get; }

    Task<int> SaveChangesAsync();
}
