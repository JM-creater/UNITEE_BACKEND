using UNITEE_BACKEND.Entities;

namespace UNITEE_BACKEND.Dto
{
    public class ProductWithSizeQuantityDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string Description { get; set; }
        public int ProductTypeId { get; set; }
        public string? Category { get; set; }
        public float Price { get; set; }
        public string? Image { get; set; }
        public bool IsActive { get; set; }

        // Relation
        public List<SizeQuantityDto> Sizes { get; set; } = new List<SizeQuantityDto>();
        public List<DepartmentDto> Departments { get; set; } = new List<DepartmentDto>();
    }
}
