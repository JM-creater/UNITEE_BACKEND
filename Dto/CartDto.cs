namespace UNITEE_BACKEND.Dto
{
    public class CartDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public List<CartItemDto> Items { get; set; }

        public CartDto()
        {
            Items = new List<CartItemDto>();
        }
    }
}
