using System.Security.Cryptography;
using System.Text;

namespace UNITEE_BACKEND.Models.Security;

public class PasswordEncryptionService
{
    public static string EncryptPassword(string password)
    {
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));    
        return Convert.ToBase64String(hashedBytes);
    }

    public static bool VerifyPassword(string enteredPassword, string storedHash)
    {
        var enteredHash = EncryptPassword(enteredPassword);
        return string.Equals(enteredHash, storedHash, StringComparison.Ordinal);
    }
}
