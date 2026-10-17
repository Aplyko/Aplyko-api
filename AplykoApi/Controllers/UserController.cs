using AplykoApi.DTO;
using AplykoApi.Models;
using AplykoApi.Utils.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplykoApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/user
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAllUsers()
    {
        var users = await _context.Users
            .Select(u => new UserResponseDto()
            {
                Id = u.Id,
                Email = u.Email,
                Role = u.Role,
                CreatedAt = u.CreatedAt
            })
            .ToListAsync();

        return Ok(users);
    }

    // GET /api/user/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponseDto>> GetUserById(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var userDto = new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };

        return Ok(userDto);
    }

    // PUT /api/user/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserRequestDto userData)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE /api/user/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
