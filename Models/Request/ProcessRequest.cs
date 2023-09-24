using UNITEE_BACKEND.Enum;

namespace UNITEE_BACKEND.Models.Request
{
    public class PlaceOrderRequest
    {
        public int Id { get; set; }
        public DateTime EstimateDate { get; set; }  
    }

    public class OrderUpdateRequest
    {
        public int Id { get; set; }
        public Status Status { get; set; }  
    }
}
