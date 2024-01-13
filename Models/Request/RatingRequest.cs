using UNITEE_BACKEND.Entities;

namespace UNITEE_BACKEND.Models.Request
{
    public class RatingRequest
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int SupplierId { get; set; }
        public int Value { get; set; }
        public string? Comment { get; set; }
    }
}
