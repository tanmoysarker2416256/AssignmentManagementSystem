using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Application.Mappings;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Services;

public class SubjectService : ISubjectService
{
    private readonly IUnitOfWork _unitOfWork;
    public SubjectService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<SubjectDto>> GetAllAsync()
    {
        var subjects = await _unitOfWork.Subjects.GetAllAsync(s => s.Class);
        return subjects.Select(s => s.ToDto());
    }

    public async Task<SubjectDto?> GetByIdAsync(int id)
    {
        var entity = await _unitOfWork.Subjects.GetByIdAsync(id, s => s.Class);
        return entity?.ToDto();
    }

    public async Task<SubjectDto> CreateAsync(CreateSubjectDto dto)
    {
        var entity = dto.ToEntity();
        await _unitOfWork.Subjects.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Subjects.GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Subject with id {id} not found.");
        _unitOfWork.Subjects.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}