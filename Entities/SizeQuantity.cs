using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UNITEE_BACKEND.Entities
{
    public class SizeQuantity
    {

        [Key]
        public int Id { get; set; }
        [ForeignKey("ProductId")]
        public int ProductId { get; set; }
        [JsonIgnore]
        public virtual Product? Product { get; set; }
        public string Size { get; set; }
        public int Quantity { get; set; }
        
    }
}
