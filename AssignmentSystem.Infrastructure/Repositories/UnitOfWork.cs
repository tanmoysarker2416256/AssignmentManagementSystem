using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Domain.Entities;
using AssignmentSystem.Infrastructure.Persistence;

namespace AssignmentSystem.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IRepository<Class> Classes { get; }
    public IRepository<Subject> Subjects { get; }
    public IRepository<TeacherSubject> TeacherSubjects { get; }
    public IRepository<Assignment> Assignments { get; }
    public IRepository<Submission> Submissions { get; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Classes = new Repository<Class>(context);
        Subjects = new Repository<Subject>(context);
        TeacherSubjects = new Repository<TeacherSubject>(context);
        Assignments = new Repository<Assignment>(context);
        Submissions = new Repository<Submission>(context);
    }

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}