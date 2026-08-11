using AssignmentSystem.Application.DTOs;

namespace AssignmentSystem.Application.Interfaces;

public interface ISubmissionService
{
    Task<SubmissionDto> SubmitOrUpdateAsync(SubmitDto dto, string studentId);
    Task<IEnumerable<SubmissionDto>> GetMySubmissionsAsync(string studentId);
    Task<IEnumerable<SubmissionDto>> GetForAssignmentAsync(int assignmentId, string teacherId);
    Task<SubmissionDto> GradeAsync(int submissionId, GradeDto dto, string teacherId);
}