using Microsoft.EntityFrameworkCore;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public class CartService : ICartService
    {
        private readonly AppDbContext context;

        public CartService(AppDbContext dbcontext) 
        {
            this.context = dbcontext;
        }

        public IEnumerable<Cart> GetAll()
            => context.Carts.AsEnumerable();

        public async Task<Cart> GetById(int id)
        {
            var e = await context.Carts.Where(a => a.Id == id).FirstOrDefaultAsync();

            if (e == null)
                throw new Exception("Cart Not Found");

            return e;
        }

        public async Task <IEnumerable<Cart>> GetCartByCustomer(int customerId)
        {
            var customer = await context.Carts  
                .Where(customer => customer.UserId == customerId)
                .ToListAsync();

            return customer;
        }

        public async Task AddToCart(int userId, int productId, int quantity, UserRole userRole)
        {
            var user = await context.Users.FindAsync(userId);
            var product = await context.Products.FindAsync(productId);

            if (user == null || product == null)
            {
                throw new Exception("User or Product not found");
            }

            if (userRole != UserRole.Customer)
            {
                throw new Exception("Only customers can add to cart");
            }

            if (product.Stocks < quantity)
            {
                throw new Exception("Insufficient stock");
            }

            var cartItem = new Cart
            {
                UserId = userId,
                ProductId = productId,
                Quantity = quantity
            };

            product.Stocks -= quantity;

            context.Carts.Add(cartItem);
            await context.SaveChangesAsync();
        }

        public async Task<Cart> Delete(int id)
        {
            var e = await this.GetById(id);
            var result = context.Remove(e);
            await Save();
            return result.Entity;
        }

        public async Task RemoveRestoreStock(int cartItemId)
        {
            var cartItem = await context.Carts.Include(c => c.Product).FirstOrDefaultAsync(c => c.Id == cartItemId);

            if (cartItem == null)
                throw new Exception("Cart item not found");

            var productId = cartItem.ProductId;
            var quantity = cartItem.Quantity;

            context.Carts.Remove(cartItem);

            var product = cartItem.Product;

            if (product != null)
            {
                product.Stocks += quantity;
            }

            await Save();
        }

        public async Task<Cart> Update(int id, CartAddRequest request)
        {
            var existingCart = await context.Carts.FindAsync(id);

            if (existingCart == null)
            {
                throw new Exception("Cart cannot found");
            }

            existingCart.Id = id;
            existingCart.UserId = request.UserId;
            existingCart.ProductId = request.ProductId;
            existingCart.Quantity = request.Quantity;

            await this.Save();

            return existingCart;
        }

        public async Task<Cart> Save(Cart request)
        {
            var e = await context.Carts.AddAsync(request);
            await this.Save();
            return e.Entity;
        }

        async Task<int> Save()
            => await context.SaveChangesAsync();

    }
}
