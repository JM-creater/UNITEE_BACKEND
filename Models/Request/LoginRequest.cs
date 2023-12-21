using System.ComponentModel.DataAnnotations;
using UNITEE_BACKEND.Enum;

namespace UNITEE_BACKEND.Models.Request
{
    public class LoginRequest
    {
        public int? Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
