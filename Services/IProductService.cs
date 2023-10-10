using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public interface IProductService
    {
        public Task<int> AddProduct(ProductRequest request);
        public IEnumerable<Product> GetAll();
        public IEnumerable<Product> GetProductsByShopId(int shopId);
        public Task<Product> GetById(int productId);
        public Task<IEnumerable<Product>> GetProductsByDepartment(int departmentId);
        public Task<Product> UpdateProduct(int productId, ProductRequest request);
        public Task<Product> Delete(int id);
        public Task<IEnumerable<ProductWithSizeQuantityDto>> GetProductsBySupplier(int supplierId);
        public Task<Product> UpdateActivationStatus(int productId, bool isActive);
    }
}
