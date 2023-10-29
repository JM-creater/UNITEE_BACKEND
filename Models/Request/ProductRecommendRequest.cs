namespace UNITEE_BACKEND.Models.Request
{
    public class ProductRecommendRequest
    {
        public string Description { get; set; }
        public string Size { get; set; }
        public int? Quantity { get; set; }
        public string Department_Name { get; set; }
        public string Product_Type { get; set; }
    }
}
