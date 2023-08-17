using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public interface IProductService
    {
        public Task<Product> AddProduct(ProductRequest request);
        public IEnumerable<Product> GetAll();
        public Task<Product> GetById(int id);
        public Task<Product> Save(Product request);
        public Task<Product> Update(int productId, ProductRequest request);
        public Task<Product> Delete(int id);
        public Task<IEnumerable<Product>> GetProductsBySupplier(int id);
        public Task<Product> UpdateActivationStatus(int productId, bool isActive);
    }
}
