﻿using System.Collections;
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
        [ForeignKey("DepartmentId")]
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public int SupplierId { get; set; }
        [ForeignKey("ProductTypeId")]
        public int ProductTypeId { get; set; }
        public virtual ProductType ProductType { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string ProductName { get; set; } = "";
        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string Description { get; set; } = "";
        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string Category { get; set; } = "";
        [Column(TypeName = "nvarchar(1000)")]
        public string? Image { get; set; }
        [Required]
        public float Price { get; set; }
        public bool IsActive { get; set; }

        // Relation
        public ICollection<SizeQuantity> Sizes { get; set; } = new List<SizeQuantity>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>(); 
    }
}
