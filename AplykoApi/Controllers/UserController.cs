using AplykoApi.DTO;
using AplykoApi.Models;
using AplykoApi.Utils.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplykoApi.Controllers;

[ApiController]
[Route("api/[controller]")]
internal sealed class UserController : ControllerBase
{
    private readonly AppDbContext _context;

    public UserController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/user
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _context.Users
            .Select(u => new UserDto
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
    public async Task<ActionResult<UserDto>> GetUserById(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };

        return Ok(userDto);
    }

    // POST /api/user
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createUser)
    {
        string normalizedEmail = createUser.Email.Trim().ToLowerInvariant();
        bool emailAlreadyExists = await _context.Users.AnyAsync(u => u.Email == normalizedEmail);

        if (emailAlreadyExists)
        {
            return Conflict(new { message = "This email is already used." });
        }

        string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(createUser.Password, workFactor: 12);

        User user = new User
        {
            Id = Guid.NewGuid(),
            Email = normalizedEmail,
            Password = hashedPassword,
            Role = UserRole.Applicant,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is Npgsql.PostgresException { SqlState: "23505" })
        {
            return Conflict(new { message = "This email is already used." });
        }

        var createdDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, createdDto);
    }

    // PUT /api/user/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UserDto dto)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        user.Email = dto.Email;
        user.Role = dto.Role;
        user.Password = dto.Password;

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
