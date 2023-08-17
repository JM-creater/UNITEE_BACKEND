using UNITEE_BACKEND.Entities;

namespace UNITEE_BACKEND.Services
{
    public interface IProductTypeService
    {
        public IEnumerable<ProductType> GetAll();
    }
}
