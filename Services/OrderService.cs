using Hangfire;
using Hangfire.Server;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Request;
using UNITEE_BACKEND.Models.SignalRNotification;

namespace UNITEE_BACKEND.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext context;
        private readonly INotificationService service;
        private readonly IHubContext<NotificationHub> hubContext;

        public OrderService(AppDbContext dbcontext, INotificationService notificationService, IHubContext<NotificationHub> _hubContext)
        {
            context = dbcontext;
            this.service = notificationService;
            hubContext = _hubContext;
        }

        public IEnumerable<Order> GetAll()
            => context.Orders
                      .Include(u => u.User)
                      .Include(c => c.Cart)
                        .ThenInclude(s => s.Supplier)
                      .Include(c => c.Cart)
                        .ThenInclude(i => i.Items)
                        .ThenInclude(p => p.Product)
                        .ThenInclude(s => s.Sizes)
                      .AsEnumerable();

        public async Task<Order?> GetById(int id)
            => await context.Orders.FirstOrDefaultAsync(o => o.Id == id);

        public async Task<List<Order>> GetAllByUserId(int id)
            => await context.Orders
                            .Include(u => u.User)
                                .ThenInclude(sr => sr.SupplierRatings)
                            .Include(u => u.Cart)
                                .ThenInclude(s => s.Supplier)
                            .Include(u => u.Cart)
                                .ThenInclude(i => i.Items)
                                    .ThenInclude(p => p.Product)
                                        .ThenInclude(s => s.Sizes)
                            .Include(u => u.Cart)
                                .ThenInclude(i => i.Items)
                                    .ThenInclude(p => p.Product)
                                        .ThenInclude(r => r.Ratings)
                                            .ThenInclude(rating => rating.User)
                            .Include(u => u.Cart)
                                .ThenInclude(u => u.Supplier)
                                    .ThenInclude(u => u.SupplierRatings)
                            .Where(o => o.UserId == id)
                            .OrderByDescending(o => o.DateCreated)
                            .ToListAsync();

        //public IEnumerable<Order> GetAllByUserId(int id)
        //{
        //    return context.Orders
        //                  .Include(o => o.User)
        //                      .ThenInclude(u => u.SupplierRatings)
        //                  .Include(o => o.Cart)
        //                      .ThenInclude(c => c.Items)
        //                          .ThenInclude(i => i.Product)
        //                              .ThenInclude(p => p.Ratings)
        //                  .Include(o => o.Cart)
        //                      .ThenInclude(c => c.Supplier)
        //                  .Where(o => o.UserId == id)
        //                  .OrderByDescending(o => o.DateCreated)
        //                  .ToList(); 
        //}

        //public async Task<List<Order>> GetAllByUserId(int id)
        //{
        //    return await context.Orders
        //        .Include(o => o.User)
        //            .ThenInclude(u => u.SupplierRatings)
        //        .Include(o => o.Cart)
        //            .ThenInclude(c => c.Items)
        //                .ThenInclude(item => item.Product)
        //                    .ThenInclude(p => p.Ratings)
        //                        .ThenInclude(rating => rating.User) 
        //        .Include(o => o.Cart)
        //            .ThenInclude(c => c.Supplier)
        //                .ThenInclude(s => s.SupplierRatings)
        //                    .ThenInclude(rating => rating.User) 
        //        .Where(o => o.UserId == id && !o.IsDeleted)
        //        .OrderByDescending(o => o.DateCreated)
        //        .ToListAsync();
        //}


        public IEnumerable<Order> GetAllTest()
        {
            var order = context.Orders.Include(o => o.User).ThenInclude(o => o.SupplierRatings).AsEnumerable();

            return order;
        }


        public async Task<List<Order>> GetAllBySupplierId(int supplierId)
        {
            return await context.Orders
                        .Include(u => u.User)
                        .Include(c => c.Cart)
                            .ThenInclude(s => s.Supplier)
                        .Include(c => c.Cart)
                            .ThenInclude(i => i.Items)
                            .ThenInclude(p => p.Product)
                            .ThenInclude(s => s.Sizes)
                        .Where(o => o.Cart.Supplier.Id == supplierId)
                        .OrderByDescending(o => o.DateCreated)
                        .ToListAsync();
        }

        public async Task<Order> GenerateReceipt(int id)
        {
            var order = await context.Orders
                                .Include(a => a.Cart)
                                    .ThenInclude(c => c.Items)
                                    .ThenInclude(c => c.Product)
                                .Where(a => a.Id == id).FirstOrDefaultAsync();

            if (order == null)
                throw new Exception("Order not found");

            return order;
        }

        public async Task<Order> AddOrder(OrderRequest request)
        {
            try
            {
                var imagePath = await ProofofPayment(request.ProofOfPayment);

                int nextId = context.Orders.Any() ? context.Orders.Max(o => o.Id) + 1 : 1;

                var order = new Order
                {
                    UserId = request.UserId,
                    CartId = request.CartId,
                    Total = request.Total,
                    ProofOfPayment = imagePath,
                    ReferenceId = request.ReferenceId,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    PaymentType = PaymentType.EMoney,
                    Status = Status.OrderPlaced,
                    OrderNumber = GenerateOrderNumber(DateTime.Now, nextId)
                };

                context.Orders.Add(order);
                await context.SaveChangesAsync();

                var notification = new Notification
                {
                    UserId = request.UserId,
                    OrderId = order.Id,
                    Message = $"Your order {order.OrderNumber} has been placed",
                    DateCreated = DateTime.Now,
                    IsRead = false,
                    UserRole = UserRole.Customer
                };

                context.Notifications.Add(notification);
                await context.SaveChangesAsync();

                var cart = await context.Carts
                                        .Include(c => c.Items)
                                        .FirstOrDefaultAsync(c => c.Id == request.CartId);

                if (cart != null)
                {
                    var supplierNotifications = new Notification
                    {
                        UserId = cart.SupplierId,
                        OrderId = order.Id,
                        Message = $"New order {order.OrderNumber} has been placed and requires your attention.",
                        DateCreated = DateTime.Now,
                        IsRead = false,
                        UserRole = UserRole.Supplier
                    };

                    context.Notifications.Add(supplierNotifications);

                    foreach (var cartItemId in request.CartItemIds)
                    {
                        var cartItem = cart.Items.FirstOrDefault(ci => ci.Id == cartItemId);

                        if (cartItem != null)
                        {
                            var orderItem = new OrderItem
                            {
                                OrderId = order.Id,
                                ProductId = cartItem.ProductId,
                                Quantity = cartItem.Quantity,
                                SizeQuantityId = cartItem.SizeQuantityId
                            };

                            context.OrderItems.Add(orderItem);
                            cartItem.IsDeleted = true;
                        }
                    }
                }

                await context.SaveChangesAsync();

                BackgroundJob.Schedule(() => UpdateOrderStatusAndNotify(order.Id, notification.Id), TimeSpan.FromSeconds(5));

                return order;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message);
            }
        }

        private string GenerateOrderNumber(DateTime dateCreated, int id)
        {
            return $"ORD-{dateCreated:yyMMdd}-{id:D5}";
        }

        public async Task UpdateOrderStatusAndNotify(int orderId, int id)
        {
            await Task.Delay(TimeSpan.FromSeconds(5));
            UpdateOrderStatusToPending(orderId, id);
        }

        public void UpdateOrderStatusToPending(int orderId, int id)
        {
            var orderToUpdate = context.Orders.Find(orderId);
            if (orderToUpdate != null && orderToUpdate.Status == Status.OrderPlaced)
            {
                orderToUpdate.Status = Status.Pending;
                orderToUpdate.DateUpdated = DateTime.Now;
                context.SaveChanges();

                hubContext.Clients.User(orderToUpdate.UserId.ToString())
                          .SendAsync("OrderStatusUpdated", "Your order status has been updated to Pending.");

                var existingNotification = context.Notifications.Where(e => e.Id == id).First();

                if (existingNotification != null) 
                {
                    existingNotification.Message = $"Your order {orderToUpdate.OrderNumber} status has been updated to Pending";
                    existingNotification.IsRead = false;
                }
                else
                {
                    var notification = new Notification
                    {
                        UserId = orderToUpdate.UserId,
                        OrderId = orderId,
                        Message = $"Your order {orderToUpdate.OrderNumber} status has been updated to Pending",
                        DateCreated = DateTime.Now,
                        IsRead = false
                    };

                    context.Notifications.Add(notification);
                }
                
                context.SaveChanges();
            }
        }

        public async Task<string?> ProofofPayment(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "ProofOfPayment");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return Path.Combine("ProofOfPayment", fileName);
        }

        public async Task<Order> ApproveOrder(int orderId)
        {
            try
            {
                var order = await context.Orders
                                         .Include(o => o.OrderItems)
                                         .ThenInclude(oi => oi.SizeQuantity)
                                         .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    throw new ArgumentException("Order not found");
                }

                if (order.Status != Status.Pending)
                {
                    throw new InvalidOperationException("Only orders with 'Pending' status can be approved");
                }

                foreach (var orderItem in order.OrderItems)
                {
                    var sizeQuantity = orderItem.SizeQuantity;

                    if (sizeQuantity != null)
                    {
                        if (sizeQuantity.Quantity >= orderItem.Quantity)
                        {
                            sizeQuantity.Quantity -= orderItem.Quantity;
                        }
                        else
                        {
                            throw new InvalidOperationException($"Insufficient stock for product {orderItem.ProductId} size {sizeQuantity.Id}");
                        }
                    }
                }

                order.Status = Status.Approved;

                var existingNotification = await context.Notifications
                                                        .Where(n => n.OrderId == order.Id)
                                                        .FirstOrDefaultAsync();

                if (existingNotification != null)
                {
                    existingNotification.Message = $"Your order {order.OrderNumber} has been approved!";
                    existingNotification.IsRead = false;
                }
                else
                {
                    var notification = new Notification
                    {
                        UserId = order.UserId,
                        OrderId = order.Id,
                        Message = $"Your order {order.OrderNumber} has been approved!",
                        DateCreated = DateTime.Now,
                        IsRead = false
                    };
                    context.Notifications.Add(notification);
                }

                await context.SaveChangesAsync();

                

                order = await context.Orders
                                    .Include(u => u.User)
                                    .Include(c => c.Cart)
                                        .ThenInclude(s => s.Supplier)
                                    .Include(c => c.Cart)
                                        .ThenInclude(i => i.Items)
                                        .ThenInclude(p => p.Product)
                                        .ThenInclude(s => s.Sizes)
                                    .Where(o => o.Id == orderId)
                                    .FirstOrDefaultAsync();

                return order;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Order> DeniedOrder(int orderId)
        {
            try
            {
                var order = await context.Orders.FindAsync(orderId);

                if (order == null)
                {
                    throw new ArgumentException("Order not found");
                }

                if (order.Status != Status.Pending)
                {
                    throw new InvalidOperationException("Only orders with 'Pending' status can be approved");
                }

                order.Status = Status.Denied;

                var existingNotification = await context.Notifications
                                                        .Where(n => n.OrderId == order.Id)
                                                        .FirstOrDefaultAsync();

                if (existingNotification != null) 
                {
                    existingNotification.Message = $"Your order {order.OrderNumber} has been denied.";
                    existingNotification.IsRead = false;
                }
                else
                {
                    var notification = new Notification
                    {
                        UserId = order.UserId,
                        OrderId = order.Id,
                        Message = $"Your order {order.OrderNumber} has been denied.",
                        DateCreated = DateTime.Now,
                        IsRead = false
                    };
                    await service.AddNotification(notification);
                }

                context.Orders.Update(order);
                await context.SaveChangesAsync();

                order = await context.Orders
                                     .Include(u => u.User)
                                     .Include(c => c.Cart)
                                         .ThenInclude(s => s.Supplier)
                                     .Include(c => c.Cart)
                                         .ThenInclude(i => i.Items)
                                         .ThenInclude(p => p.Product)
                                         .ThenInclude(s => s.Sizes)
                                     .Where(o => o.Id == orderId)
                                     .FirstOrDefaultAsync();

                return order;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Order> CanceledOrder(int orderId)
        {
            try
            {
                var order = await context.Orders.FindAsync(orderId);

                if (order == null) 
                {
                    throw new ArgumentException("Order not found");
                }

                if (order.Status != Status.Pending)
                {
                    throw new InvalidOperationException("Only orders with 'Pending' status can be approved");
                }

                order.Status = Status.Canceled;

                var existingNotification = await context.Notifications
                                                        .Where(o => o.OrderId == order.Id)
                                                        .FirstOrDefaultAsync();

                if (existingNotification != null)
                {
                    existingNotification.Message = $"Your order {order.OrderNumber} is canceled.";
                    existingNotification.IsRead = false;
                }
                else
                {
                    var notification = new Notification
                    {
                        UserId = order.UserId,
                        OrderId = order.Id,
                        Message = $"Your order {order.OrderNumber} is canceled.",
                        DateCreated = DateTime.Now,
                        IsRead = false
                    };
                    await service.AddNotification(notification);
                }

                order.IsDeleted = true;

                context.Orders.Update(order);
                await context.SaveChangesAsync();

                order = await context.Orders
                                     .Include(u => u.User)
                                     .Include(c => c.Cart)
                                         .ThenInclude(s => s.Supplier)
                                     .Include(c => c.Cart)
                                         .ThenInclude(i => i.Items)
                                         .ThenInclude(p => p.Product)
                                         .ThenInclude(s => s.Sizes)
                                     .Where(o => o.Id == orderId)
                                     .FirstOrDefaultAsync();

                return order;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<Order> ForPickUp(int orderId)
        {
            try
            {
                var order = await context.Orders.FindAsync(orderId);

                if (order == null)
                {
                    throw new ArgumentException("Order not found");
                }

                if (order.Status != Status.Approved)
                {
                    throw new InvalidOperationException("Only orders with 'Approved' status can be approved");
                }

                var pickUpDate = DateTime.Now.AddDays(5);

                order.Status = Status.ForPickUp;

                order.EstimateDate = pickUpDate;

                var existingNotification = await context.Notifications
                                                        .Where(o => o.OrderId == order.Id)
                                                        .FirstOrDefaultAsync();

                if (existingNotification != null)
                {
                    existingNotification.Message = $"Your order {order.OrderNumber} is ready for pick-up. The pick-up date is on {pickUpDate:d}.";
                    existingNotification.IsRead = false;
                }
                else
                {
                    var notification = new Notification
                    {
                        UserId = order.UserId,
                        OrderId = order.Id,
                        Message = $"Your order {order.OrderNumber} is ready for pick-up. The pick-up date is on {pickUpDate:d}.",
                        DateCreated = DateTime.Now,
                        IsRead = false
                    };
                    await service.AddNotification(notification);
                }

                context.Orders.Update(order);
                await context.SaveChangesAsync();

                order = await context.Orders
                                     .Include(u => u.User)
                                     .Include(c => c.Cart)
                                         .ThenInclude(s => s.Supplier)
                                     .Include(c => c.Cart)
                                         .ThenInclude(i => i.Items)
                                         .ThenInclude(p => p.Product)
                                         .ThenInclude(s => s.Sizes)
                                     .Where(o => o.Id == orderId)
                                     .FirstOrDefaultAsync();

                return order;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }


        public async Task<Order> CompletedOrder(int orderId)
        {
            try
            {
                var order = await context.Orders.FindAsync(orderId);

                if (order == null)
                {
                    throw new ArgumentException("Order not found");
                }

                if (order.Status != Status.ForPickUp)
                {
                    throw new InvalidOperationException("Only orders with 'ForPickUp' status can be marked as completed");
                }

                order.Status = Status.Completed;

                var completionDate = DateTime.Now;

                var existingNotification = await context.Notifications
                    .Where(o => o.OrderId == order.Id)
                    .FirstOrDefaultAsync();

                if (existingNotification != null)
                {
                    existingNotification.Message = $"Your order {order.OrderNumber} has been completed. Thank you for shopping with us!";
                    existingNotification.IsRead = false;
                }
                else
                {
                    var notification = new Notification
                    {
                        UserId = order.UserId,
                        OrderId = order.Id,
                        Message = $"Your order {order.OrderNumber} has been completed. Thank you for shopping with us!",
                        DateCreated = DateTime.Now,
                        IsRead = false
                    };
                    await service.AddNotification(notification);
                }

                context.Orders.Update(order);
                await context.SaveChangesAsync();

                return order;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }
    }
}
