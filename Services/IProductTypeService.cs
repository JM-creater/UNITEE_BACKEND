using UNITEE_BACKEND.Entities;

namespace UNITEE_BACKEND.Services
{
    public interface IProductTypeService
    {
        public Task<IEnumerable<ProductType>> GetAll();
    }
}
