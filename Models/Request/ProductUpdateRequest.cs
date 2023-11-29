using UNITEE_BACKEND.Dto;

namespace UNITEE_BACKEND.Models.Request
{
    public class ProductUpdateRequest
    {
        public int SupplierId { get; set; }
        public int ProductTypeId { get; set; }
        public int DepartmentId { get; set; }
        public string ProductName { get; set; } = "";
        public string Description { get; set; } = "";
        public string Category { get; set; } = "";
        public float Price { get; set; }
        public IFormFile? Image { get; set; }
        public IFormFile? FrontViewImage { get; set; }
        public IFormFile? SideViewImage { get; set; }
        public IFormFile? BackViewImage { get; set; }
        public IFormFile? SizeGuide { get; set; }
        public List<SizeQuantityDto> Sizes { get; set; } = new List<SizeQuantityDto>();
        public List<int> DepartmentIds { get; set; } = new List<int>();
    }
}
