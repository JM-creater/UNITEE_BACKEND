﻿namespace UNITEE_BACKEND.Entities
{
    public class Rating
    {
        public int RatingId { get; set; }   
        public int UserId { get; set; }
        public virtual User User { get; set; }
        //public int ProductId { get; set; }
        //public virtual Product Product { get; set; }
        //public int SupplierId { get; set; } 
        //public virtual User Supplier { get; set; }
        public int Value { get; set; }
        public DateTime DateCreated { get; set; }
    }
}