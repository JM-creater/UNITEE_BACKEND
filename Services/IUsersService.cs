using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public interface IUsersService
    {
        public Task<User> RegisterCustomer(RegisterRequest request);
        public Task<User> RegisterSupplier(SupplierRequest request);
        public Task<(User user, UserRole role)> Login(LoginRequest request);
        public Task<User> ConfirmEmail(string confirmationCode);
        public Task<User> VerifyLater(int userId);
        public Task<User> VerifyEmail(int userId);
        public Task<User?> GetTopSellingSeller();
        public IEnumerable<User> GetAll();
        public Task<User> SupplierById(int id);
        public IEnumerable<User> GetAllSuppliers();
        public Task<List<User>> GetSupplierById(int id);
        public IEnumerable<Product> GetProductsBySupplierShop(int supplierId);
        public IEnumerable<User> GetAllCustomers();
        public IEnumerable<User> GetAllSuppliersProducts(int departmentId);
        public Task<User> GetById(int id);
        public Task<User> ValidateCustomer(int id, ValidateUserRequest request);
        public Task<User> ValidateSupplier(int id, ValidateUserRequest request);
        public Task<User> UpdateCustomerProfile(int id, UpdateCustomerRequest request);
        public Task<User> UpdateAdminProfile(int id, UpdateAdminRequest request);
        public Task<User> UpdateProfileSupplier(int id, UpdateSupplierRequest request);
        public Task<User> UpdateCustomerPassword(int id, UpdatePasswordRequest request);
        public Task<User> UpdateSupplierPassword(int id, UpdatePasswordRequest request);
        public Task SendEmailAsync(string email, string subject, string message);
        public Task<User> ForgotPassword(string email);
        public Task<User> ResetPassword(ResetPasswordDto dto);
        public Task<bool> IsResetTokenValid(string token);
    }
}
