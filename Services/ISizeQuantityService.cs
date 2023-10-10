using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public interface ISizeQuantityService
    {
        public IEnumerable<SizeQuantity> GetAll();
        public Task<SizeQuantity> AddSizeQuantity(SizeRequest request);
        public Task<SizeQuantity> Save(SizeQuantity request);
        public Task<IEnumerable<SizeQuantity>> GetByProductId(int productId);
        public Task<Dictionary<string, int>> GetSizeQuantitiesForProduct(int productId);
        public Task<SizeQuantity> Update(int id, UpdateSizeQuantityDto dto);
        public Task<SizeQuantity> Create(CreateSizeQuantityDto dto);
        public Task<bool> DeleteSizeQuantity(int id);
        public Task<IEnumerable<GetSizeQuantityByIdDto>> GetSizesByProductId(int productId);
        public Task<SizeQuantity> AddSizeToProduct(int productId, SizeQuantityDto dto);
        public Task<bool> DeleteSizeFromProduct(int productId, string sizeLabel);
        public Task<SizeQuantity> UpdateQuantity(int productId, string size, int newQuantity);
    }
}
