using Azure.Core;
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
        {
            var cart = context.Carts.Include(c => c.Items).AsEnumerable();

            return cart;
        }


        public async Task<Cart?> GetById(int id)
            => await context.Carts.Where(c => c.Id == id).FirstOrDefaultAsync();

        public async Task<List<Cart>> GetByUserId(int id)
        {
            var user = await context.Users
                .Include(u => u.Carts)
                    .ThenInclude(c => c.Items)
                        .ThenInclude(i => i.Product)
                .Include(u => u.Carts)
                    .ThenInclude(c => c.Supplier)
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
                
            return user.Carts.ToList();
        }

        public async Task<List<Cart>> GetMyCart(int userId)
        {
            try
            {
                return await GetByUserId(userId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task <IEnumerable<Cart>> GetCartByCustomer(int customerId)
            => await GetByUserId(customerId);

        public async Task AddToCart(UserRole userRole, CartAddRequest request)
        {
            try
            {
                var user = await context.Users.Include(u => u.Carts).Where(u => u.Id == request.UserId).FirstOrDefaultAsync();

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
                //sizeQuantity.Quantity -= request.Quantity;

                var cart = user.Carts.Where(c => c.SupplierId == product.SupplierId).FirstOrDefault();

                if (cart != null)
                {
                    cart.Items.Add(new CartItem
                    {
                        ProductId = request.ProductId,
                        SizeQuantityId = sizeQuantity.Id,
                        Quantity = request.Quantity,
                    });
                    context.Carts.Update(cart);
                }
                else
                {
                    context.Carts.Add(new Cart
                    {
                        UserId = request.UserId,
                        SupplierId = product.SupplierId,
                        Items = new List<CartItem> {
                            new CartItem
                            {
                                ProductId = request.ProductId,
                                SizeQuantityId = sizeQuantity.Id,
                                Quantity = request.Quantity,
                            }
                        }
                    });
                }


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
            //existingCart.UserId = request.UserId;
            //existingCart.ProductId = request.ProductId;
            //existingCart.Quantity = request.Quantity;

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
