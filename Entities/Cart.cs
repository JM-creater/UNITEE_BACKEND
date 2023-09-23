using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UNITEE_BACKEND.Entities
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int ProductTypeId { get; set; }
        public string? ProductName { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public string? Size { get; set; }
        public string? Image { get; set; }
    }
}
