using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Application.Interfaces;
using AssignmentSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AssignmentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubmissionsController : ControllerBase
{
    private readonly ISubmissionService _submissionService;
    public SubmissionsController(ISubmissionService submissionService) => _submissionService = submissionService;

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<SubmissionDto>> SubmitOrUpdate(SubmitDto dto)
    {
        try
        {
            return Ok(await _submissionService.SubmitOrUpdateAsync(dto, CurrentUserId));
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }

    [HttpGet("my")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<IEnumerable<SubmissionDto>>> GetMine() =>
        Ok(await _submissionService.GetMySubmissionsAsync(CurrentUserId));

    [HttpGet("assignment/{assignmentId}")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<IEnumerable<SubmissionDto>>> GetForAssignment(int assignmentId)
    {
        try
        {
            return Ok(await _submissionService.GetForAssignmentAsync(assignmentId, CurrentUserId));
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
    }

    [HttpPatch("{id}/grade")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<SubmissionDto>> Grade(int id, GradeDto dto)
    {
        try
        {
            return Ok(await _submissionService.GradeAsync(id, dto, CurrentUserId));
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
        catch (ArgumentOutOfRangeException ex) { return BadRequest(ex.Message); }
    }
}