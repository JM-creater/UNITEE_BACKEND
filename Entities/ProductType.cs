using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UNITEE_BACKEND.Entities
{
    public class ProductType
    {
        [Key]
        public int ProductTypeId { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string Product_Type { get; set; }
    }
}
