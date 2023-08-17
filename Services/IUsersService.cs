using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public interface IUsersService
    {
        public IEnumerable<User> GetAll();
        public IEnumerable<User> GetAllSuppliers();
        public Task<User> GetById(int id);
        public Task<User> Save(User request);
        public Task<User> Update(User request);
        public Task<User> Delete(int id);
        public Task<User> Register(RegisterRequest request);
        public Task<(User user, UserRole role)> Login(LoginRequest request);
    }
}
