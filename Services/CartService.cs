﻿using Microsoft.EntityFrameworkCore;
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

        public async Task<IEnumerable<Cart>> GetAll()
            => await context.Carts
                            .Include(c => c.Items)
                            .ToListAsync();

        public async Task<Cart?> GetById(int id)
            => await context.Carts
                            .Include(c => c.Items)
                            .Where(c => c.Id == id)
                            .FirstOrDefaultAsync();

        public async Task<List<Cart>> GetByUserId(int id)
        {
            try
            {
                var user = await context.Users
                                    .Where(u => u.Id == id)
                                    .FirstOrDefaultAsync();

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
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
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
            int supplierId = await context.Products.Where(p => p.ProductId == request.ProductId).Select(p => p.SupplierId).FirstOrDefaultAsync();

            var cart = await context.Carts
                                    .Include(c => c.Items)
                                    .ThenInclude(ci => ci.SizeQuantity)
                                    .FirstOrDefaultAsync(c => c.UserId == request.UserId &&
                                                                c.SupplierId == supplierId);

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

            var sizeQuantity = await context.SizeQuantities
                                            .Where(sq => sq.ProductId == request.ProductId &&
                                                          sq.Size == request.Size)
                                            .FirstOrDefaultAsync();

            if (sizeQuantity == null)
            {
                throw new InvalidOperationException("Product size not found.");
            }

            var existingCartItem = cart.Items
                .FirstOrDefault(i => i.ProductId == request.ProductId &&
                                       i.SizeQuantityId == sizeQuantity.Id);

            if (existingCartItem != null)
            {
                if (existingCartItem.IsDeleted)
                {
                    existingCartItem.IsDeleted = false;
                    existingCartItem.Quantity = request.Quantity; 
                }
                else if (existingCartItem.Quantity + request.Quantity > sizeQuantity.Quantity)
                {
                    throw new InvalidOperationException("Requested quantity exceeds available quantity.");
                }
                else
                {
                    existingCartItem.Quantity += request.Quantity;
                }
            }
            else
            {
                if (request.Quantity > sizeQuantity.Quantity)
                {
                    throw new InvalidOperationException("Requested quantity exceeds available quantity.");
                }

                var cartItem = new CartItem
                {
                    ProductId = request.ProductId,
                    SizeQuantityId = sizeQuantity.Id,
                    Quantity = request.Quantity,
                    IsDeleted = false
                };

                cart.Items.Add(cartItem);
            }

            await context.SaveChangesAsync();
        }




        public async Task DeleteCart(int id)
        {
            var cart = await context.Carts.FindAsync(id);
                
            if (cart != null)
            {
                cart.IsDeleted = true;
                await context.SaveChangesAsync();
            }
        }

        public async Task<CartItem> Delete(int cartId, int itemId)
        {
            try
            {
                var cartItem = await context.CartItems
                                            .Include(ci => ci.SizeQuantity)
                                            .FirstOrDefaultAsync(ci => ci.Id == itemId && ci.CartId == cartId); 

                if (cartItem == null)
                {
                    throw new InvalidOperationException($"No cart item found with Id: {itemId} in cart {cartId}");
                }

                context.CartItems.Remove(cartItem);
                await context.SaveChangesAsync();

                return cartItem;
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
