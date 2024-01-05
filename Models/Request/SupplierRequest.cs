using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        public IFormFile? BIR { get; set; }
        public IFormFile? CityPermit { get; set; }
        public IFormFile? SchoolPermit { get; set; }
        public IFormFile? BarangayClearance { get; set; }
        public IFormFile? ValidIdFrontImage { get; set; }
        public IFormFile? ValidIdBackImage { get; set; }
    }
}
