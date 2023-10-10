namespace UNITEE_BACKEND.Dto
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public float Price { get; set; } 
        public int Quantity { get; set; }
    }
}
