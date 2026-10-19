using AplykoApi.DTO;
using AplykoApi.Models;
using AplykoApi.Utils.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AplykoApi.Services;

namespace AplykoApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public sealed class UserController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordUtils _passwordUtils;

    public UserController(AppDbContext context)
    {
        _context = context;
        _passwordUtils = new PasswordUtils();
    }

    /// <summary>
    /// Return the list of every user in the database, including their email, role, and creation date. This endpoint is protected and requires authentication.
    /// </summary>
    /// <response code="200">Returns the list of users</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <returns></returns>
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

    /// <summary>
    /// Return the details of a specific user by their ID, including their email, role, and creation date. This endpoint is protected and requires authentication.
    /// </summary>
    /// <response code="200">Return the user by its ID</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <returns></returns>
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

    /// <summary>
    /// Update the details of a specific user by their ID. This endpoint is protected and requires authentication.
    /// </summary>
    /// <response code="204">If the user was successfully updated</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <param name="id"></param>
    /// <param name="userData">The fields that need to be updated for the user</param>
    /// <returns></returns>
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

    /// <summary>
    /// This endpoint is to edit user password
    /// </summary>
    /// <response code="200">If the password was changed successfully</response>
    /// <response code="400">If the old password is incorrect</response>
    /// <response code="404">If the user was not found</response>
    /// <param name="id"></param>
    /// <param name="requestData"></param>
    /// <returns></returns>
    // PATCH /api/user/{id}/edit-password
    [HttpPatch("{id:guid}/edit-password")]
    public async Task<IActionResult> EditUserPassword(Guid id, [FromBody] EditPasswordRequestDto requestData)
    {
        if (requestData.NewPassword == requestData.OldPassword)
        {
            return BadRequest(new { message = "The new password cannot be the same as the old password" });
        }

        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return NotFound(new { message = "No user were found with this ID" });
        }

        bool isOldPasswordValid = _passwordUtils.VerifyPassword(requestData.OldPassword, user.Password);
        if (!isOldPasswordValid)
        {
            return BadRequest(new { message = "The old password is incorrect" });
        }

        user.Password = _passwordUtils.HashPassword(requestData.NewPassword);

        await _context.SaveChangesAsync();

        return Ok(new { message = "The password was changed successfully" });
    }

    /// <summary>
    /// Delete a specific user by their ID. This endpoint is protected and requires authentication.
    /// </summary>
    /// <response code="204">If the user was successfully deleted</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <param name="id"></param>
    /// <returns></returns>
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
