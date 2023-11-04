using System.ComponentModel.DataAnnotations;

namespace UNITEE_BACKEND.Entities
{
    public class ProductDepartment
    {
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; } 
    }
}
