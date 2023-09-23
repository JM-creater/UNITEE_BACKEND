using System.ComponentModel.DataAnnotations;

namespace UNITEE_BACKEND.Entities
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int SizeQuantityId { get; set; }
        public int Quantity { get; set; }   

    }
}
