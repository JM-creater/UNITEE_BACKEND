using System.ComponentModel.DataAnnotations;
using UNITEE_BACKEND.Enum;

namespace UNITEE_BACKEND.Entities
{
    public class Notification
    {
        [Key] 
        public int Id { get; set; }
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public virtual Order Order { get; set; }
        public string Message { get; set; }
        public DateTime DateCreated { get; set; } 
        public DateTime DateUpdated { get; set; }
        public bool IsRead { get; set; }
        public UserRole UserRole { get; set; }
    }
}
