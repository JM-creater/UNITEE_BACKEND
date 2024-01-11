using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UNITEE_BACKEND.Enum;

namespace UNITEE_BACKEND.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public int CartId { get; set; }
        public virtual Cart Cart { get; set; }
        [Column(TypeName = "nvarchar(50)")]
        public string OrderNumber { get; set; }
        [Column(TypeName = "nvarchar(13)")]
        public string? ReferenceId { get; set; }
        public string? ProofOfPayment { get; set; }
        public DateTime? EstimatedDate { get; set; }
        public float Total { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public PaymentType PaymentType { get; set; }
        public Status Status { get; set; }
        public bool IsDeleted { get; set; } = false;
        public string? CancellationReason { get; set; }
        public bool IsReceived { get; set; }

        public virtual ICollection<OrderItem>? OrderItems { get; set; }
        public virtual ICollection<Notification>? Notifications { get; set; }
    }
}
