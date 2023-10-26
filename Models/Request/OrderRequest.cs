using UNITEE_BACKEND.Enum;

namespace UNITEE_BACKEND.Models.Request
{
    public class OrderRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CartId { get; set; }
        public int SupplierId { get; set; }
        public List<int> CartItemIds { get; set; }
        public List<OrderItemRequest> OrderItems { get; set; }
        public IFormFile? ProofOfPayment { get; set; }
        public string? ReferenceId { get; set; }
        public float Total { get; set; }
    }

}
