namespace UNITEE_BACKEND.Models.Request
{
    public class OrderItemRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int SizeQuantityId { get; set; }
    }
}
