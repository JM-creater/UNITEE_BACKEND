using System.ComponentModel.DataAnnotations;
using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Enum;

namespace UNITEE_BACKEND.Models.Request
{
    public class ProductRequest
    {
        public int SupplierId { get; set; }
        public int ProductTypeId { get; set; }
        public int DepartmentId { get; set; }
        public List<int> DepartmentIds { get; set; }
        public string ProductName { get; set; } = "";
        public string Description { get; set; } = "";
        public string Category { get; set; } = "";
        public float Price { get; set; }
        public IFormFile? Image { get; set; }
        public List<SizeQuantityDto> Sizes { get; set; } = new List<SizeQuantityDto>();
    }
}
