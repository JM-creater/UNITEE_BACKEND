using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public interface ISizeQuantityService
    {
        public Task<IEnumerable<SizeQuantity>> GetAll();
        public Task<SizeQuantity> AddSizeQuantity(SizeRequest request);
        public Task<IEnumerable<SizeQuantity>> GetByProductId(int productId);
        public Task<Dictionary<string, int>> GetSizeQuantitiesForProduct(int productId);
        public Task<SizeQuantity> Update(int id, UpdateSizeQuantityDto dto);
        public Task<SizeQuantity> Create(CreateSizeQuantityDto dto);
        public Task<IEnumerable<GetSizeQuantityByIdDto>> GetSizesByProductId(int productId);
        public Task<SizeQuantity> AddSizeToProduct(int productId, SizeQuantityDto dto);
        public Task<SizeQuantity> UpdateQuantity(int id, int productId, string size, int newQuantity);
        public Task<SizeQuantity> DeleteSizeQuantity(int id);
    }
}
