using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Application.Mappings;
using AssignmentSystem.Domain.Entities;


namespace AssignmentSystem.Application.Services;

public class ClassService : IClassService
{
    private readonly IUnitOfWork _unitOfWork;
    public ClassService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<IEnumerable<ClassDto>> GetAllAsync()
    {
        var classes = await _unitOfWork.Classes.GetAllAsync();
        return classes.Select(c => c.ToDto());
    }

    public async Task<ClassDto?> GetByIdAsync(int id)
    {
        var entity = await _unitOfWork.Classes.GetByIdAsync(id);
        return entity?.ToDto();
    }

    public async Task<ClassDto> CreateAsync(CreateClassDto dto)
    {
        var entity = dto.ToEntity();
        await _unitOfWork.Classes.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return entity.ToDto();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Classes.GetByIdAsync(id);
        if (entity == null)
            throw new KeyNotFoundException($"Class with id {id} not found.");
        _unitOfWork.Classes.Remove(entity);
        await _unitOfWork.SaveChangesAsync();
    }
}