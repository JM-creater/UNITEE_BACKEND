using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UNITEE_BACKEND.Entities
{
    public class ProductType
    {
        [Key]
        public int ProductTypeId { get; set; }
        public string Product_Type { get; set; }
    }
}
