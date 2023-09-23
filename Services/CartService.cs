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

        public async Task AddToCart(UserRole userRole, CartAddRequest request)
        {
            try
            {
                var user = await context.Users.FindAsync(request.UserId);

                if (user == null)
                    throw new Exception($"User with ID {request.UserId} not found");

                if (userRole != UserRole.Customer)
                    throw new Exception("Only customer can add to cart");

                var product = await context.Products
                                           .Include(p => p.Sizes)
                                           .FirstOrDefaultAsync(p => p.ProductId == request.ProductId);

                if (product == null)
                    throw new Exception($"Product with ID {request.ProductId} not found");

                if (userRole != UserRole.Customer)
                    throw new Exception("Only customers can add to cart");

                var sizeQuantity = product.Sizes.FirstOrDefault(s => s.Size == request.Size);
                if (sizeQuantity == null)
                    throw new Exception($"Size {request.Size} not available for this product");

                if (sizeQuantity.Quantity < request.Quantity)
                    throw new Exception($"Insufficient stock for size {request.Size}");

                sizeQuantity.Quantity -= request.Quantity;

                var cartItem = new Cart
                {
                    UserId = request.UserId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Size = request.Size,
                    ProductTypeId = request.ProductTypeId,
                    ProductName = request.ProductName,
                    Price = request.Price,
                    Image = request.Image,
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
