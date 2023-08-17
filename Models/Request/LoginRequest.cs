using System.ComponentModel.DataAnnotations;
using UNITEE_BACKEND.Enum;

namespace UNITEE_BACKEND.Models.Request
{
    public class LoginRequest
    {
        public int Id { get; set; }
        public string Password { get; set; } = "";
    }
}
