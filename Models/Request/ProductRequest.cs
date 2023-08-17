using System.ComponentModel.DataAnnotations;
using UNITEE_BACKEND.Enum;

namespace UNITEE_BACKEND.Models.Request
{
    public class ProductRequest
    {
        public int SupplierId { get; set; }
        public int ProductTypeId { get; set; }
        public int DepartmentId { get; set; }
        [Required]
        public string ProductName { get; set; } = "";
        [Required]
        public string Description { get; set; } = "";
        [Required]
        public string Category { get; set; } = "";
        [Required]
        public string? Sizes { get; set; }
        [Required]
        public float Price { get; set; }
        [Required]
        public int Stocks { get; set; }
        public IFormFile? Image { get; set; }
    }
}
