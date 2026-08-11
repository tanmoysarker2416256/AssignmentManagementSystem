using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AssignmentSystem.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUnitOfWork _unitOfWork;

    private static readonly string[] AllowedRoles = { "Admin", "Teacher", "Student" };

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = _userManager.Users.ToList();
        var result = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            result.Add(new UserDto(user.Id, user.FullName, user.Email!, roles.FirstOrDefault() ?? "", user.ClassId));
        }
        return result;
    }

    public async Task<IEnumerable<UserDto>> GetByRoleAsync(string role)
    {
        if (!AllowedRoles.Contains(role))
            throw new ArgumentException($"Invalid role '{role}'.");

        var users = await _userManager.GetUsersInRoleAsync(role);
        return users.Select(u => new UserDto(u.Id, u.FullName, u.Email!, role, u.ClassId));
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        if (!AllowedRoles.Contains(dto.Role))
            throw new ArgumentException($"Invalid role '{dto.Role}'. Must be Admin, Teacher, or Student.");

        if (dto.Role == "Student" && dto.ClassId == null)
            throw new ArgumentException("ClassId is required when creating a Student.");

        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
            throw new InvalidOperationException("A user with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
            EmailConfirmed = true,
            ClassId = dto.Role == "Student" ? dto.ClassId : null
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

        await _userManager.AddToRoleAsync(user, dto.Role);
        return new UserDto(user.Id, user.FullName, user.Email!, dto.Role, user.ClassId);
    }

    public async Task DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new KeyNotFoundException("User not found.");
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
    }

    public async Task AssignTeacherToSubjectAsync(AssignTeacherDto dto)
    {
        var teacher = await _userManager.FindByIdAsync(dto.TeacherId)
            ?? throw new KeyNotFoundException("Teacher not found.");
        var roles = await _userManager.GetRolesAsync(teacher);
        if (!roles.Contains("Teacher"))
            throw new ArgumentException("Specified user is not a Teacher.");

        var subject = await _unitOfWork.Subjects.GetByIdAsync(dto.SubjectId)
            ?? throw new KeyNotFoundException("Subject not found.");

        var already = await _unitOfWork.TeacherSubjects.FindAsync(
            ts => ts.TeacherId == dto.TeacherId && ts.SubjectId == dto.SubjectId);
        if (already.Any())
            throw new InvalidOperationException("Teacher is already assigned to this subject.");

        await _unitOfWork.TeacherSubjects.AddAsync(new TeacherSubject
        {
            TeacherId = dto.TeacherId,
            SubjectId = dto.SubjectId
        });
        await _unitOfWork.SaveChangesAsync();
    }
}