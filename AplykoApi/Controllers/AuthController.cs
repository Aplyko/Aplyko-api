using System.Security.Claims;
using AplykoApi.DTO;
using AplykoApi.Utils.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AplykoApi.Controllers;

[ApiController]
[Route("/api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuthController(AppDbContext context)
    {
        _context = context;
    }

    // POST /api/auth/login
    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        string normalizedEmail = dto.Email.Trim().ToLowerInvariant();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

        if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(dto.Password, user.Password))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        List<Claim> claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        AuthenticationProperties authProperties = new AuthenticationProperties
        {
            IsPersistent = dto.RememberMe,
            ExpiresUtc = dto.RememberMe ? DateTimeOffset.UtcNow.AddDays(14) : null
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties
        );

        UserResponseDto userDto = new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };

        return Ok(userDto);
    }

    // POST /api/auth/register
    [HttpPost("[action]")]
    public async Task<ActionResult<UserResponseDto>> Register(RegisterRequestDto register)
    {
        string normalizedEmail = register.Email.Trim().ToLowerInvariant();
        bool emailAlreadyExists = await _context.Users.AnyAsync(u => u.Email == normalizedEmail);

        if (emailAlreadyExists)
        {
            return Conflict(new { message = "This email is already used." });
        }

        string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(register.Password, workFactor: 12);

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

        UserResponseDto createdDto = new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };


        return CreatedAtAction(nameof(UserController.GetUserById), "User", new { id = user.Id }, createdDto);
    }

    [Authorize]
    [HttpGet("[action]")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { message = "Logged out successfully." });
    }
}
