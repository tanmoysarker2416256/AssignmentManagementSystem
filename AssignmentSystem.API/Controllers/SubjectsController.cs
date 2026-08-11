using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;
    public SubjectsController(ISubjectService subjectService) => _subjectService = subjectService;

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<SubjectDto>>> GetAll() =>
        Ok(await _subjectService.GetAllAsync());

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SubjectDto>> GetById(int id)
    {
        var result = await _subjectService.GetByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<SubjectDto>> Create(CreateSubjectDto dto)
    {
        var created = await _subjectService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _subjectService.DeleteAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("my")]
    [Authorize(Roles = "Teacher")]
    public async Task<ActionResult<IEnumerable<SubjectDto>>> GetMine()
    {
        var teacherId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)!;
        return Ok(await _subjectService.GetForTeacherAsync(teacherId));
    }
}