﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UNITEE_BACKEND.Entities
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string Department_Name { get; set; }
        public ICollection<ProductDepartment> ProductDepartments { get; set; } = new List<ProductDepartment>();
    }
}
