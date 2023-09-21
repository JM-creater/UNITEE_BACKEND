using System.ComponentModel.DataAnnotations;

namespace UNITEE_BACKEND.Dto
{
    public class UpdateProductDto
    {
        public int ProductTypeId { get; set; }
        public int DepartmentId { get; set; }
        public string ProductName { get; set; } = "";
        public string Description { get; set; } = "";
        public string Category { get; set; } = "";
        public float Price { get; set; }
        public IFormFile? Image { get; set; }
        public List<SizeQuantityDto> Sizes { get; set; } = new List<SizeQuantityDto>();
    }
}
