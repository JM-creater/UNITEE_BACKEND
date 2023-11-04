using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Enum;

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
        public string ProductName { get; set; } 
        public string Description { get; set; }
        public string Category { get; set; }
        public string? Image { get; set; }
        public float Price { get; set; }
        public bool IsActive { get; set; }

        // Relation
        public ICollection<SizeQuantity> Sizes { get; set; } = new List<SizeQuantity>();
        public ICollection<ProductDepartment> ProductDepartments { get; set; } = new List<ProductDepartment>();
    }
}
