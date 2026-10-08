public record UserDto
(
    int Id,
    string Uuid,
    string Email,
    string Password,
    UserRole UserRole,
    string FirstName,
    string LastName,
    DateOnly CreatedAt
);