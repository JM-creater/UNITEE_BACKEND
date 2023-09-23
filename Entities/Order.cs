using UNITEE_BACKEND.Enum;

namespace UNITEE_BACKEND.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CartId { get; set; }
        public virtual Cart Cart { get; set; }
        public string? ReferenceId { get; set; }
        public string? ProofOfPayment { get; set; }
        public float Total { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public DateTime? EstimateDate { get; set; }
        public PaymentType PaymentType { get; set; }
        public Status Status { get; set; }
    }
}
