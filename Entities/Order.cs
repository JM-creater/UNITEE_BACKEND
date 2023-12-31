﻿using System.ComponentModel.DataAnnotations;
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
        public string OrderNumber { get; set; }
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
