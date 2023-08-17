using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public interface ICartService
    {
        public IEnumerable<Cart> GetAll();
        public Task<Cart> GetById(int id);
        public Task<IEnumerable<Cart>> GetCartByCustomer(int customerId);
        public Task<Cart> Delete(int id);
        public Task<Cart> Save(Cart request);
        public Task AddToCart(int userId, int productId, int quantity, UserRole userRole);
        public Task RemoveRestoreStock(int cartItemId);
        public Task<Cart> Update(int id, CartAddRequest request);
    }
}
