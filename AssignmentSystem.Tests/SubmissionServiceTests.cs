using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Application.Services;
using AssignmentSystem.Domain.Entities;
using Moq;
using Xunit;

namespace AssignmentSystem.Tests;

public class SubmissionServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IRepository<Submission>> _submissionRepoMock;
    private readonly Mock<IRepository<Assignment>> _assignmentRepoMock;
    private readonly SubmissionService _sut; // "system under test"  common naming convention

    public SubmissionServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _submissionRepoMock = new Mock<IRepository<Submission>>();
        _assignmentRepoMock = new Mock<IRepository<Assignment>>();

        _unitOfWorkMock.Setup(u => u.Submissions).Returns(_submissionRepoMock.Object);
        _unitOfWorkMock.Setup(u => u.Assignments).Returns(_assignmentRepoMock.Object);

        _sut = new SubmissionService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task SubmitOrUpdateAsync_FirstSubmissionAfterDeadline_MarksAsLate()
    {
        // arrange
        var assignment = new Assignment
        {
            Id = 1,
            Status = AssignmentStatus.Published,
            Deadline = DateTime.UtcNow.AddDays(-1), // deadline was yesterday
            MaxMarks = 100
        };
        _assignmentRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(assignment);
        _submissionRepoMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Submission, bool>>>()))
            .ReturnsAsync(new List<Submission>()); // no existing submission

        var dto = new SubmitDto(1, "My answer");

        // act
        var result = await _sut.SubmitOrUpdateAsync(dto, "student-1");

        // assert
        Assert.Equal("Late", result.Status);
    }

    [Fact]
    public async Task SubmitOrUpdateAsync_UpdateAfterDeadline_ThrowsInvalidOperationException()
    {
        var assignment = new Assignment
        {
            Id = 1,
            Status = AssignmentStatus.Published,
            Deadline = DateTime.UtcNow.AddDays(-1),
            MaxMarks = 100
        };
        var existingSubmission = new Submission { Id = 5, AssignmentId = 1, StudentId = "student-1" };

        _assignmentRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(assignment);
        _submissionRepoMock
            .Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Submission, bool>>>()))
            .ReturnsAsync(new List<Submission> { existingSubmission });

        var dto = new SubmitDto(1, "Updated answer");

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.SubmitOrUpdateAsync(dto, "student-1"));
    }

    [Fact]
    public async Task SubmitOrUpdateAsync_UnpublishedAssignment_ThrowsInvalidOperationException()
    {
        var assignment = new Assignment { Id = 1, Status = AssignmentStatus.Draft };
        _assignmentRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(assignment);

        var dto = new SubmitDto(1, "Answer");

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _sut.SubmitOrUpdateAsync(dto, "student-1"));
    }

    [Fact]
    public async Task GradeAsync_MarksExceedMaxMarks_ThrowsArgumentOutOfRangeException()
    {
        var assignment = new Assignment { Id = 1, TeacherId = "teacher-1", MaxMarks = 100 };
        var submission = new Submission { Id = 5, Assignment = assignment, AssignmentId = 1 };

        _submissionRepoMock
            .Setup(r => r.GetByIdAsync(5, It.IsAny<System.Linq.Expressions.Expression<Func<Submission, object>>[]>()))
            .ReturnsAsync(submission);

        var dto = new GradeDto(150, "Too high"); // exceeds MaxMarks of 100

        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(
            () => _sut.GradeAsync(5, dto, "teacher-1"));
    }

    [Fact]
    public async Task GradeAsync_WrongTeacher_ThrowsUnauthorizedAccessException()
    {
        var assignment = new Assignment { Id = 1, TeacherId = "teacher-1", MaxMarks = 100 };
        var submission = new Submission { Id = 5, Assignment = assignment, AssignmentId = 1 };

        _submissionRepoMock
            .Setup(r => r.GetByIdAsync(5, It.IsAny<System.Linq.Expressions.Expression<Func<Submission, object>>[]>()))
            .ReturnsAsync(submission);

        var dto = new GradeDto(80, "Good");

        // "teacher-2" is NOT the owning teacher ("teacher-1")
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _sut.GradeAsync(5, dto, "teacher-2"));
    }
}