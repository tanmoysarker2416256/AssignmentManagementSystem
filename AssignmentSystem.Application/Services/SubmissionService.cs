using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Application.Mappings;
using AssignmentSystem.Domain.Entities;

namespace AssignmentSystem.Application.Services;

public class SubmissionService : ISubmissionService
{
    private readonly IUnitOfWork _unitOfWork;
    public SubmissionService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<SubmissionDto> SubmitOrUpdateAsync(SubmitDto dto, string studentId)
    {
        var assignment = await _unitOfWork.Assignments.GetByIdAsync(dto.AssignmentId)
            ?? throw new KeyNotFoundException("Assignment not found.");

        if (assignment.Status != AssignmentStatus.Published)
            throw new InvalidOperationException("Cannot submit to an unpublished assignment.");

        var existing = (await _unitOfWork.Submissions.FindAsync(
            s => s.AssignmentId == dto.AssignmentId && s.StudentId == studentId)).FirstOrDefault();

        var now = DateTime.UtcNow;

        if (existing == null)
        {
            var submission = new Submission
            {
                AssignmentId = dto.AssignmentId,
                StudentId = studentId,
                Content = dto.Content,
                SubmittedAt = now,
                Status = now > assignment.Deadline ? SubmissionStatus.Late : SubmissionStatus.Submitted
            };
            await _unitOfWork.Submissions.AddAsync(submission);
            await _unitOfWork.SaveChangesAsync();
            return submission.ToDto();
        }
        else
        {
            if (now > assignment.Deadline)
                throw new InvalidOperationException("Cannot update a submission after the deadline.");

            existing.Content = dto.Content;
            existing.SubmittedAt = now;
            _unitOfWork.Submissions.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return existing.ToDto();
        }
    }

    public async Task<IEnumerable<SubmissionDto>> GetMySubmissionsAsync(string studentId)
    {
        var submissions = await _unitOfWork.Submissions.FindAsync(
            s => s.StudentId == studentId,
            s => s.Assignment, s => s.Student);
        return submissions.Select(s => s.ToDto());
    }

    public async Task<IEnumerable<SubmissionDto>> GetForAssignmentAsync(int assignmentId, string teacherId)
    {
        var assignment = await _unitOfWork.Assignments.GetByIdAsync(assignmentId)
            ?? throw new KeyNotFoundException("Assignment not found.");
        if (assignment.TeacherId != teacherId)
            throw new UnauthorizedAccessException("You can only view submissions for your own assignments.");

        var submissions = await _unitOfWork.Submissions.FindAsync(
            s => s.AssignmentId == assignmentId,
            s => s.Assignment, s => s.Student);
        return submissions.Select(s => s.ToDto());
    }

    public async Task<SubmissionDto> GradeAsync(int submissionId, GradeDto dto, string teacherId)
    {
        var submission = await _unitOfWork.Submissions.GetByIdAsync(
            submissionId, s => s.Assignment, s => s.Student)
            ?? throw new KeyNotFoundException("Submission not found.");

        if (submission.Assignment.TeacherId != teacherId)
            throw new UnauthorizedAccessException("You can only grade submissions for your own assignments.");

        if (dto.Marks < 0 || dto.Marks > submission.Assignment.MaxMarks)
            throw new ArgumentOutOfRangeException(
                nameof(dto),
                $"Marks must be between 0 and {submission.Assignment.MaxMarks}.");

        submission.Marks = dto.Marks;
        submission.Feedback = dto.Feedback;
        submission.Status = SubmissionStatus.Graded;
        submission.GradedAt = DateTime.UtcNow;

        _unitOfWork.Submissions.Update(submission);
        await _unitOfWork.SaveChangesAsync();
        return submission.ToDto();
    }
}