using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UNITEE_BACKEND.Entities
{
    public class Department
    {
        [Key]
        public int DepartmentId { get; set; }
        public string Department_Name { get; set; }
    }
}
