using AssignmentSystem.Application.DTOs;

namespace AssignmentSystem.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<IEnumerable<UserDto>> GetByRoleAsync(string role);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task DeleteAsync(string id);
    Task AssignTeacherToSubjectAsync(AssignTeacherDto dto);
}