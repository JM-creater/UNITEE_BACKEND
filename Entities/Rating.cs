using System.ComponentModel.DataAnnotations.Schema;
using UNITEE_BACKEND.Enum;

namespace UNITEE_BACKEND.Entities
{
    public class Rating
    {
        public int RatingId { get; set; }
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public int ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
        public int SupplierId { get; set; }

        [ForeignKey("SupplierId")]
        public virtual User Supplier { get; set; }
        public int Value { get; set; }
        public DateTime DateCreated { get; set; }
        public RatingRole Role { get; set; }
    }
}
