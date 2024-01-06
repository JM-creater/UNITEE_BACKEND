using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public interface IProductService
    {
        public Task<int> AddProduct(ProductRequest request);
        public Task<IEnumerable<Product>> RecommendProducts(string search);
        public Task<IEnumerable<Product>> RecommendProductsPurchase(int id);
        public Task<IEnumerable<Product>> GetSearchProductByUserDepartment(int userId);
        public Task<int> GetQuantityBySize(int productId, int sizeQuantityId);
        public Task<IEnumerable<Product>> GetTopSellingProducts(int topCount);
        public Task<IEnumerable<Product>> GetTopSellingProductsByShop(int shopId, int topCount);
        public Task<IEnumerable<Product>> GetAll();
        public Task<IEnumerable<Product>> GetProductsByShopId(int shopId);
        public Task<IEnumerable<Product>> GetProductsByShopIdAndDepartmentId(int shopId, int departmentId);
        public Task<Product> GetById(int productId);
        public Task<IEnumerable<Product>> GetProductsByDepartment(int departmentId);
        public Task<Product> UpdateProduct(int productId, ProductUpdateRequest request);
        public Task<Product> Delete(int id);
        public Task<IEnumerable<ProductWithSizeQuantityDto>> GetProductsBySupplier(int supplierId);
        public Task<Product> UpdateActivationStatus(int productId);
        public Task<Product> UpdateDectivationStatus(int productId);
    }
}
