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
            => context.Carts
                      .Include(c => c.Items)
                      .AsEnumerable();

        public async Task<Cart?> GetById(int id)
            => await context.Carts
                            .Include(c => c.Items)
                            .Where(c => c.Id == id)
                            .FirstOrDefaultAsync();

        public async Task<List<Cart>> GetByUserId(int id)
        {
            var user = await context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

            if (user == null)
                throw new Exception($"User with Id {id} not found");

            user.Carts = await context.Carts
                                .Include(c => c.Supplier)
                                .Include(c => c.Items)
                                    .ThenInclude(i => i.Product)
                                .Include(c => c.Items)
                                    .ThenInclude(i => i.SizeQuantity)
                                        .ThenInclude(i => i.Product)
                                            .ThenInclude(i => i.Sizes)
                                .Where(c => c.UserId == user.Id && c.Items.Any(i => !i.IsDeleted)) 
                                .OrderByDescending(c => c.DateCreated)
                                .ToListAsync();

            foreach (var cart in user.Carts)
            {
                cart.Items = cart.Items
                                 .Where(i => !i.IsDeleted)
                                 .ToList();
            }

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
                throw new ArgumentException(e.Message);
            }
        }

        public async Task <IEnumerable<Cart>> GetCartByCustomer(int customerId)
            => await GetByUserId(customerId);

        public async Task AddToCart(UserRole userRole, CartAddRequest request)
        {
            int supplierId = context.Products.First(p => p.ProductId == request.ProductId).SupplierId;

            var cart = await context.Carts
                                    .Include(c => c.Items)
                                    .ThenInclude(ci => ci.SizeQuantity)
                                    .FirstOrDefaultAsync(c => c.UserId == request.UserId &&
                                                                c.SupplierId == supplierId &&
                                                                !c.IsDeleted &&
                                                                !context.Orders.Any(o => o.CartId == c.Id));

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = request.UserId,
                    SupplierId = supplierId,
                    Items = new List<CartItem>(),
                    DateCreated = DateTime.Now
                };

                context.Carts.Add(cart);
            }

            var cartItem = cart.Items
                                .FirstOrDefault(i => i.ProductId == request.ProductId &&
                                                        i.SizeQuantity.Size == request.Size);

            if (cartItem != null)
            {
                if (cartItem.Quantity + request.Quantity > cartItem.SizeQuantity.Quantity)
                {
                    throw new InvalidOperationException("Requested quantity exceeds available quantity.");
                }

                cartItem.Quantity += request.Quantity;
            }
            else
            {
                var sizeQuantity = await context.SizeQuantities
                                                .Where(sq => sq.ProductId == request.ProductId &&
                                                                sq.Size == request.Size)
                                                .FirstOrDefaultAsync();

                if (sizeQuantity == null)
                {
                    throw new InvalidOperationException("Product size not found.");
                }

                if (request.Quantity > sizeQuantity.Quantity)
                {
                    throw new InvalidOperationException("Requested quantity exceeds available quantity.");
                }

                cartItem = new CartItem
                {
                    ProductId = request.ProductId,
                    SizeQuantityId = sizeQuantity.Id,
                    Quantity = request.Quantity
                };

                cart.Items.Add(cartItem);
            }

            await context.SaveChangesAsync();
        }

        public async Task DeleteCart(int id)
        {
            var cart = context.Carts.Find(id);
                
            if (cart != null)
            {
                cart.IsDeleted = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task<Cart> Delete(int id)
        {
            try
            {
                var cart = await context.Carts
                                        .Include(c => c.Items) 
                                        .FirstOrDefaultAsync(c => c.Id == id);

                if (cart == null)
                {
                    throw new KeyNotFoundException($"No cart found with the ID: {id}");
                }

                if (cart.Items != null && cart.Items.Any())
                {
                    context.CartItems.RemoveRange(cart.Items);
                }

                context.Carts.Remove(cart);
                await context.SaveChangesAsync();

                return cart;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<Cart> Update(int id, CartAddRequest request)
        {
            try
            {
                var existingCart = await context.Carts.FindAsync(id);

                if (existingCart == null)
                {
                    throw new Exception("Cart cannot found");
                }

                existingCart.Id = id;

                context.Carts.Update(existingCart);
                await context.SaveChangesAsync();

                return existingCart;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }
    }
}
