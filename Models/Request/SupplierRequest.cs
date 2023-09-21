﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UNITEE_BACKEND.Models.Request
{
    public class SupplierRequest
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string ShopName { get; set; } = "";
        public string Address { get; set; } = "";
        public IFormFile? Image { get; set; }
    }
}
