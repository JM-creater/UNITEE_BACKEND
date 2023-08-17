namespace UNITEE_BACKEND.Models.Request
{
    public class CartAddRequest
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
