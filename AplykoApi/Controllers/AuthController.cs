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

    /// <summary>
    /// Authenticates a user and returns their data if successful. If the email or password is incorrect, it returns an Unauthorized response.
    /// </summary>
    /// <param name="userData">User's email and password, user can indicate if he wants the browser to keep the cookie for 14 days</param>
    /// <response code="200">Returns the user's data if authentication is successful</response>
    /// <response code="401">If the email or password is incorrect</response>
    /// <returns>The user's data</returns>
    // POST /api/auth/login
    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto userData)
    {
        string normalizedEmail = userData.Email.Trim().ToLowerInvariant();
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail);

        if (user == null || !BCrypt.Net.BCrypt.EnhancedVerify(userData.Password, user.Password))
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
            IsPersistent = userData.RememberMe,
            ExpiresUtc = userData.RememberMe ? DateTimeOffset.UtcNow.AddDays(14) : null
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

    /// <summary>
    /// Create a new user account. If the email is already used, it returns a Conflict response.
    /// </summary>
    /// <response code="201">Returns the created user's data if registration is successful</response>
    /// <response code="409">If the email is already used</response>
    /// <param name="userData"></param>
    /// <returns></returns>
    // POST /api/auth/register
    [HttpPost("[action]")]
    public async Task<ActionResult<UserResponseDto>> Register(RegisterRequestDto userData)
    {
        string normalizedEmail = userData.Email.Trim().ToLowerInvariant();
        bool emailAlreadyExists = await _context.Users.AnyAsync(u => u.Email == normalizedEmail);

        if (emailAlreadyExists)
        {
            return Conflict(new { message = "This email is already used." });
        }

        string hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(userData.Password, workFactor: 12);

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
