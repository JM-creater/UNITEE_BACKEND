using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public interface ISupplierService
    {
        public Task<User> GetSupplierById(int id);
        public Task<User> Save(User request);
        public Task<User> AddSupplier(SupplierRequest request);
        public Task<User> UpdateSupplier(int id, SupplierRequest request);
        public Task<User> ActivateSupplier(int id, bool isActive);
    }
}
