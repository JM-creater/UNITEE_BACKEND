using System.ComponentModel.DataAnnotations.Schema;
using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;

namespace UNITEE_BACKEND.Models.Request
{
    public class CartAddRequest
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int ProductTypeId { get; set; }
        public string ProductName { get; set; }
        public float Price { get; set; }
        public string Size { get; set; } 
        public int Quantity { get; set; }
        public string? Image { get; set; }
    }
}
