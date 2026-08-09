using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AssignmentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AssignmentsController : ControllerBase
{
    private readonly IAssignmentService _assignmentService;
    public AssignmentsController(IAssignmentService assignmentService) => _assignmentService = assignmentService;

    private string CurrentUserId =>
        User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;

    [HttpGet("my")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetMine() =>
        Ok(await _assignmentService.GetForTeacherAsync(CurrentUserId));

    [HttpGet("available")]
    [Authorize(Roles = "Student")]
    public async Task<ActionResult<IEnumerable<AssignmentDto>>> GetAvailable() =>
        Ok(await _assignmentService.GetForStudentAsync(CurrentUserId));

    [HttpGet("{id}")]
    public async Task<ActionResult<AssignmentDto>> GetById(int id)
    {
        var result = await _assignmentService.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<AssignmentDto>> Create(CreateAssignmentDto dto)
    {
        try
        {
            var created = await _assignmentService.CreateAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(403, ex.Message);
        }
    }

    [HttpPatch("{id}/publish")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Publish(int id)
    {
        try
        {
            await _assignmentService.PublishAsync(id, CurrentUserId);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Teacher")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _assignmentService.DeleteAsync(id, CurrentUserId);
            return NoContent();
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (UnauthorizedAccessException ex) { return StatusCode(403, ex.Message); }
    }
}