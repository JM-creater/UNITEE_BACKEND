using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UNITEE_BACKEND.Entities
{
    public class ProductType
    {
        [Key]
        public int ProductTypeId { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(250)")]
        public string Product_Type { get; set; } = "";
    }
}
