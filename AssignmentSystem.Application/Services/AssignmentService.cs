using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Application.Mappings;
using AssignmentSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AssignmentSystem.Application.Services;

public class AssignmentService : IAssignmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public AssignmentService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<IEnumerable<AssignmentDto>> GetForTeacherAsync(string teacherId)
    {
        var assignments = await _unitOfWork.Assignments.FindAsync(
            a => a.TeacherId == teacherId,
            a => a.Subject, a => a.Teacher);
        return assignments.Select(a => a.ToDto());
    }

    public async Task<IEnumerable<AssignmentDto>> GetForStudentAsync(string studentId)
    {
        var student = await _userManager.FindByIdAsync(studentId);
        if (student?.ClassId == null)
            return Enumerable.Empty<AssignmentDto>(); // no class assigned = nothing to show

        var assignments = await _unitOfWork.Assignments.FindAsync(
            a => a.Status == AssignmentStatus.Published && a.Subject.ClassId == student.ClassId,
            a => a.Subject, a => a.Teacher);
        return assignments.Select(a => a.ToDto());
    }

    public async Task<AssignmentDto?> GetByIdAsync(int id)
    {
        var entity = await _unitOfWork.Assignments.GetByIdAsync(id, a => a.Subject, a => a.Teacher);
        return entity?.ToDto();
    }

    public async Task<AssignmentDto> CreateAsync(CreateAssignmentDto dto, string teacherId)
    {
        var teaches = await _unitOfWork.TeacherSubjects.FindAsync(
            ts => ts.TeacherId == teacherId && ts.SubjectId == dto.SubjectId);
        if (!teaches.Any())
            throw new InvalidOperationException("You are not assigned to teach this subject.");

        var entity = dto.ToEntity(teacherId);
        await _unitOfWork.Assignments.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.ToDto();
    }

    public async Task PublishAsync(int id, string teacherId)
    {
        var entity = await _unitOfWork.Assignments.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Assignment with id {id} not found.");
        if (entity.TeacherId != teacherId)
            throw new UnauthorizedAccessException("You can only publish your own assignments.");

        entity.Status = AssignmentStatus.Published;
        _unitOfWork.Assignments.Update(entity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id, string teacherId)
    {
        var entity = await _unitOfWork.Assignments.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Assignment with id {id} not found.");
        if (entity.TeacherId != teacherId)
            throw new UnauthorizedAccessException("You can only delete your own assignments.");

        _unitOfWork.Assignments.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}