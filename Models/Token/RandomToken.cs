using System.Security.Cryptography;

namespace UNITEE_BACKEND.Models.Token
{
    public class RandomToken
    {
        public static string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }
    }
}
