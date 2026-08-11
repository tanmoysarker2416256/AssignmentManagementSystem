using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.DTOs;

public record CreateUserDto(string FullName, string Email, string Password, string Role, int? ClassId);
public record UserDto(string Id, string FullName, string Email, string Role, int? ClassId);
public record AssignTeacherDto(string TeacherId, int SubjectId);
