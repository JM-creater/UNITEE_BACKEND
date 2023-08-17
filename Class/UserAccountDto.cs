using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;

namespace UNITEE_BACKEND.Class
{
    public class UserAccountDto
    {
        public User? User { get; set; }
        public UserRole Role { get; set; }
    }
}
