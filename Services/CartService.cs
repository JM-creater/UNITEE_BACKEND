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

        public List<Cart> GetMyCart(int userId)
        {
            try
            {
                return context.Carts.Where(cart => cart.UserId == userId).ToList();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task <IEnumerable<Cart>> GetCartByCustomer(int customerId)
        {
            var customer = await context.Carts  
                .Where(customer => customer.UserId == customerId)
                .ToListAsync();

            return customer;
        }

        public async Task AddToCart(int userId, int productId, string size, int quantity, UserRole userRole)
        {
            try
            {
                var user = await context.Users.FindAsync(userId);
                var product = await context.Products
                                            .Include(p => p.Sizes)
                                            .FirstOrDefaultAsync(p => p.ProductId == productId);

                if (user == null)
                    throw new Exception($"User with ID {userId} not found");

                if (product == null)
                    throw new Exception($"Product with ID {productId} not found");

                if (userRole != UserRole.Customer)
                    throw new Exception("Only customers can add to cart");

                var sizeQuantity = product.Sizes.FirstOrDefault(s => s.Size == size);
                if (sizeQuantity == null)
                    throw new Exception($"Size {size} not available for this product");

                if (sizeQuantity.Quantity < quantity)
                    throw new Exception($"Insufficient stock for size {size}");

                sizeQuantity.Quantity -= quantity;

                var cartItem = new Cart
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = quantity,
                    Size = size
                };

                context.Carts.Add(cartItem);
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Cart> Delete(int id)
        {
            var e = await this.GetById(id);
            var result = context.Remove(e);
            await Save();
            return result.Entity;
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
