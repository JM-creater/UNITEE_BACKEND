using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public interface ISupplierService
    {
        public Task<User> GetSupplierById(int id);
        public Task<User> RegisterSupplier(SupplierRequest request);
        public Task<User> UpdatePassword(int id, UpdatePasswordRequest request);
        public Task<User> UpdateSupplier(int id, SupplierRequest request);
    }
}
