using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UNITEE_BACKEND.Entities
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        [ForeignKey("DepartmentId")]
        public int? DepartmentId { get; set; }
        public virtual Department? Department { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string FirstName { get; set; } = "";
        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string LastName { get; set; } = "";
        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string Password { get; set; } = "";
        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string Email { get; set; } = "";
        [Required]
        [Column(TypeName = "nvarchar(50)")]
        public string PhoneNumber { get; set; } = "";
        [Column(TypeName = "nvarchar(10)")]
        public string Gender { get; set; } = "";
        [Column(TypeName = "nvarchar(250)")]
        public string ShopName { get; set; } = "";
        [Column(TypeName = "nvarchar(250)")]
        public string Address { get; set; } = "";
        [Required]
        [Column(TypeName = "nvarchar(1000)")]
        public string? Image { get; set; }
        [Column(TypeName = "nvarchar(1000)")]
        public string? StudyLoad { get; set; }
        [Column(TypeName = "nvarchar(1000)")]
        public string? BIR { get; set; }
        [Column(TypeName = "nvarchar(1000)")]
        public string? CityPermit { get; set; }
        [Column(TypeName = "nvarchar(1000)")]
        public string? SchoolPermit { get; set; }
        public bool IsActive { get; set; } = true;
        public int Role { get; set; }
        public bool IsValidate { get; set; } = false;

        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public ICollection<Rating> SupplierRatings { get; set; } = new List<Rating>();  
    }
}
