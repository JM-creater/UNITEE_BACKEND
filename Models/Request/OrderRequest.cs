using UNITEE_BACKEND.Enum;

namespace UNITEE_BACKEND.Models.Request
{
    public class OrderRequest
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CartId { get; set; }
        public float Total { get; set; }
    }

}
