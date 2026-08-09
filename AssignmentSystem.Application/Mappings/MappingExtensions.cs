using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Domain.Entities;

namespace AssignmentSystem.Application.Mappings;

public static class MappingExtensions
{
    public static ClassDto ToDto(this Class entity) =>
        new(entity.Id, entity.Name);

    public static Class ToEntity(this CreateClassDto dto) =>
        new() { Name = dto.Name };

    public static SubjectDto ToDto(this Subject entity) =>
        new(entity.Id, entity.Name, entity.ClassId, entity.Class?.Name ?? string.Empty);

    public static Subject ToEntity(this CreateSubjectDto dto) =>
        new() { Name = dto.Name, ClassId = dto.ClassId };


    public static AssignmentDto ToDto(this Assignment entity) => new(
    entity.Id,
    entity.Title,
    entity.Description,
    entity.Deadline,
    entity.MaxMarks,
    entity.Status.ToString(),
    entity.CreatedAt,
    entity.SubjectId,
    entity.Subject?.Name ?? string.Empty,
    entity.TeacherId,
    entity.Teacher?.FullName ?? string.Empty
);

    public static Assignment ToEntity(this CreateAssignmentDto dto, string teacherId) => new()
    {
        Title = dto.Title,
        Description = dto.Description,
        SubjectId = dto.SubjectId,
        Deadline = dto.Deadline,
        MaxMarks = dto.MaxMarks,
        TeacherId = teacherId,
        Status = AssignmentStatus.Draft // always starts as Draft — publishing is separate
    };

}