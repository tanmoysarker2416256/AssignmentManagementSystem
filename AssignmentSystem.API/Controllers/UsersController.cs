using AssignmentSystem.Application.DTOs;
using AssignmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssignmentSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAll() =>
        Ok(await _userService.GetAllAsync());

    [HttpGet("role/{role}")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetByRole(string role)
    {
        try { return Ok(await _userService.GetByRoleAsync(role)); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create(CreateUserDto dto)
    {
        try { return Ok(await _userService.CreateAsync(dto)); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) { return Conflict(ex.Message); }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try { await _userService.DeleteAsync(id); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
    }

    [HttpPost("assign-teacher")]
    public async Task<IActionResult> AssignTeacher(AssignTeacherDto dto)
    {
        try { await _userService.AssignTeacherToSubjectAsync(dto); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) { return Conflict(ex.Message); }
    }
}