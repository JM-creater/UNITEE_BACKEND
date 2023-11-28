using System.Security.Cryptography;
using System.Text;

namespace UNITEE_BACKEND.Models.Token
{
    public class RandomToken
    {
        public static string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }

        public static string GenerateConfirmationCode()
        {
            int length = 10;

            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            StringBuilder sb = new();
            Random random = new();

            while (0 < length--)
            {
                sb.Append(validChars[random.Next(validChars.Length)]);
            }

            return sb.ToString();
        }
    }
}
