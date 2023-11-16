using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public interface IUsersService
    {
        public IEnumerable<User> GetAll();
        public IEnumerable<User> GetAllSuppliers();
        public Task<List<User>> GetSupplierById(int id);
        public IEnumerable<Product> GetProductsBySupplierShop(int supplierId);
        public IEnumerable<User> GetAllCustomers();
        public IEnumerable<User> GetAllSuppliersProducts(int departmentId);
        public Task<User> GetById(int id);
        public Task<User> Update(int id, UpdateCustomerRequest request);
        public Task<User> UpdateSupplier(int id, UpdateSupplierRequest request);
        public Task<User> Register(RegisterRequest request);
        public Task<(User user, UserRole role)> Login(LoginRequest request);
        public Task<User> ValidateUser(int id, ValidateUserRequest request);
        public Task<User> ValidateSupplier(int id, ValidateUserRequest request);
        public Task<User> UpdatePassword(int id, UpdatePasswordRequest request);
    }
}
