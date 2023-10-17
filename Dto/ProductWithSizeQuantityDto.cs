using UNITEE_BACKEND.Entities;

namespace UNITEE_BACKEND.Dto
{
    public class ProductWithSizeQuantityDto
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public int ProductTypeId { get; set; }
        public int DepartmentId { get; set; }
        public string? Category { get; set; }
        public float Price { get; set; }
        public string? Image { get; set; }
        public bool IsActive { get; set; }

        // Relation
        public IEnumerable<SizeQuantityDto>? Sizes { get; set; }
    }
}
