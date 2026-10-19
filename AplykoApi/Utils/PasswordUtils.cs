namespace AplykoApi.Services;

// This service is here to make other files cleaner
public class PasswordUtils
{
    public string HashPassword(string notHashedPassword)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(notHashedPassword, 12);
    }

    public bool VerifyPassword(string passwordToCompare, string actualPassword)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(passwordToCompare, actualPassword);
    }
}
