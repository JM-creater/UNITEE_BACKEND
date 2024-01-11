using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UNITEE_BACKEND.Entities
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public virtual User Supplier { get; set; }
        public int ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }
        public int? RatingId { get; set; }
        public virtual Rating Rating { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string ProductName { get; set; } 
        public string Description { get; set; }
        [Column(TypeName = "nvarchar(10)")]
        public string Category { get; set; }
        public string? Image { get; set; }
        public string? FrontViewImage { get; set; }
        public string? SideViewImage { get; set; }
        public string? BackViewImage { get; set; }
        public string? SizeGuide { get; set; }
        public float Price { get; set; }
        public bool IsActive { get; set; }

        public ICollection<SizeQuantity> Sizes { get; set; } = new List<SizeQuantity>();
        public ICollection<ProductDepartment> ProductDepartments { get; set; } = new List<ProductDepartment>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}
