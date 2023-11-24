using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;

namespace UNITEE_BACKEND.Entities
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public virtual User Supplier { get; set; }
        public int UserId { get; set; }
        public ICollection<CartItem>? Items { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime DateCreated { get; set; }
    }
}
