using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public interface ICartService
    {
        public Task AddToCart(UserRole userRole, CartAddRequest request);
        public IEnumerable<Cart> GetAll();
        public Task<Cart> GetById(int id);
        public Task<List<Cart>> GetMyCart(int userId);
        public Task<List<Cart>> GetByUserId(int userId);
        public Task<IEnumerable<Cart>> GetCartByCustomer(int customerId);
        public Task DeleteCart(int id);
        public Task<CartItem> Delete(int cartId, int itemId);
        public Task<Cart> Update(int id, CartAddRequest request);
    }
}
