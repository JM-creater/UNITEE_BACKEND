using System.ComponentModel.DataAnnotations.Schema;
using UNITEE_BACKEND.Enum;

namespace UNITEE_BACKEND.Entities
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        public int? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        public int? RatingId { get; set; }
        public virtual Rating Rating { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? ShopName { get; set; }
        public string? Address { get; set; }
        public string? Image { get; set; }
        public string? StudyLoad { get; set; }
        public string? BIR { get; set; }
        public string? CityPermit { get; set; }
        public string? SchoolPermit { get; set; }
        public string? BarangayClearance { get; set; }
        public string? ValidIdFrontImage { get; set; } 
        public string? ValidIdBackImage { get; set; }
        public bool IsActive { get; set; }
        public int Role { get; set; }
        public bool IsValidate { get; set; }
        public EmailStatus EmailVerificationStatus { get; set; }
        public DateTime EmailVerificationSentTime { get; set; }
        public string? ConfirmationCode { get; set; }
        public string? EmailConfirmationToken { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
