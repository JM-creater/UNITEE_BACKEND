﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UNITEE_BACKEND.Entities
{
    public class SizeQuantity
    {

        [Key]
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }
        [Column(TypeName = "nvarchar(15)")]
        public string Size { get; set; }
        public int Quantity { get; set; }
        
    }
}
