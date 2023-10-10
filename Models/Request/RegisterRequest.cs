using System.ComponentModel.DataAnnotations;

namespace UNITEE_BACKEND.Models.Request
{
    public class RegisterRequest
    {
        [Required]
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        [Required]
        public string FirstName { get; set; } = "";
        [Required]
        public string LastName { get; set; } = "";
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
        [Required]
        public string PhoneNumber { get; set; } = "";
        [Required]
        public string Gender { get; set; } = "";
        [Required]
        public IFormFile? Image { get; set; }
        [Required]
        public IFormFile? StudyLoad { get; set; }
    }
}
