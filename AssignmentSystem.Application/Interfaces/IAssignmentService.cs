using AssignmentSystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentSystem.Application.Interfaces;

public interface IAssignmentService
{
    Task<IEnumerable<AssignmentDto>> GetForTeacherAsync(string teacherId);
    Task<IEnumerable<AssignmentDto>> GetForStudentAsync(string studentId);
    Task<AssignmentDto?> GetByIdAsync(int id);
    Task<AssignmentDto> CreateAsync(CreateAssignmentDto dto, string teacherId);
    Task PublishAsync(int id, string teacherId);
    Task DeleteAsync(int id, string teacherId);
}
