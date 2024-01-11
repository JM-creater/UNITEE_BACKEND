using Hangfire;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Email;
using UNITEE_BACKEND.Models.ImageDirectory;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext context;
        private readonly INotificationService service;
        private readonly IConfiguration configuration;

        public OrderService(AppDbContext dbcontext, INotificationService notificationService, IConfiguration _configuration)
        {
            context = dbcontext;
            service = notificationService;
            configuration = _configuration;
        }

        public async Task<IEnumerable<Order>> GetAll()
            => await context.Orders
                      .Include(u => u.User)
                      .Include(c => c.Cart)
                        .ThenInclude(s => s.Supplier)
                      .Include(c => c.Cart)
                        .ThenInclude(i => i.Items)
                        .ThenInclude(p => p.Product)
                        .ThenInclude(s => s.Sizes)
                       .Include(o => o.OrderItems)
                      .OrderByDescending(o => o.DateCreated)
                      .ToListAsync();

        public async Task<Order?> GetById(int id)
            => await context.Orders
                            .Where(o => o.Id == id)
                            .FirstOrDefaultAsync();

        public async Task<List<Order>> GetAllByUserId(int id)
            => await context.Orders
                            .Include(u => u.Cart)
                                .ThenInclude(s => s.Supplier)
                            .Include(u => u.Cart)
                                .ThenInclude(i => i.Items)
                                    .ThenInclude(p => p.Product)
                                        .ThenInclude(s => s.Sizes)
                            .Include(u => u.Cart)
                                .ThenInclude(i => i.Items)
                                    .ThenInclude(p => p.Product)
                            .Include(u => u.Cart)
                                .ThenInclude(u => u.Supplier)
                            .Include(u => u.User)
                            .Include(u => u.OrderItems)
                            .Where(o => o.UserId == id)
                            .OrderByDescending(o => o.DateCreated)
                            .ToListAsync();

        public async Task<List<Order>> GetAllBySupplierId(int supplierId)
            => await context.Orders
                        .Include(u => u.User)
                        .Include(c => c.Cart)
                            .ThenInclude(s => s.Supplier)
                        .Include(c => c.Cart)
                            .ThenInclude(i => i.Items)
                            .ThenInclude(p => p.Product)
                            .ThenInclude(s => s.Sizes)
                        .Include(oi => oi.OrderItems)
                        .Where(o => o.Cart.Supplier.Id == supplierId)
                        .OrderByDescending(o => o.DateCreated)
                        .ToListAsync();

        public async Task<IEnumerable<float>> GetWeeklySales(DateTime startDate, int supplierId)
        {
            try
            {
                var endDate = startDate.AddDays(7);

                return await context.Orders
                                    .Where(o => o.DateCreated >= startDate &&
                                                o.DateCreated < endDate &&
                                                o.Status == Status.Completed &&
                                                o.Cart.SupplierId == supplierId)
                                    .Select(o => o.Total)
                                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<IEnumerable<float>> GetMonthlySales(int year, int month, int supplierId)
        {
            try
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1);

                return await context.Orders
                                    .Where(o => o.DateCreated >= startDate &&
                                                o.DateCreated < endDate &&
                                                o.Status == Status.Completed &&
                                                o.Cart.SupplierId == supplierId)
                                    .Select(o => o.Total)
                                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<IEnumerable<float>> GetYearlySales(int year, int supplierId)
        {
            try
            {
                var startDate = new DateTime(year, 1, 1);
                var endDate = startDate.AddYears(1);

                var yearlySales = await context.Orders
                                               .Where(o => o.DateCreated >= startDate &&
                                                           o.DateCreated < endDate &&
                                                           o.Status == Status.Completed &&
                                                           o.Cart.SupplierId == supplierId)
                                               .GroupBy(o => new { o.DateCreated.Year, o.DateCreated.Month })
                                               .Select(g => new { Month = g.Key.Month, Total = g.Sum(o => o.Total) })
                                               .OrderBy(g => g.Month)
                                               .Select(g => g.Total)
                                               .ToListAsync();

                return yearlySales;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<IEnumerable<float>> GetWeeklySalesAdmin(DateTime startDate)
        {
            try
            {
                var endDate = startDate.AddDays(7);

                return await context.Orders
                                    .Where(o => o.DateCreated >= startDate &&
                                                o.DateCreated < endDate &&
                                                o.Status == Status.Completed)
                                    .Select(o => o.Total)
                                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<IEnumerable<float>> GetMonthlySalesAdmin(int year, int month)
        {
            try
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1);

                return await context.Orders
                                    .Where(o => o.DateCreated >= startDate &&
                                                o.DateCreated < endDate &&
                                                o.Status == Status.Completed)
                                    .Select(o => o.Total)
                                    .ToListAsync();
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<IEnumerable<float>> GetYearlySalesAdmin(int year)
        {
            try
            {
                var startDate = new DateTime(year, 1, 1);
                var endDate = startDate.AddYears(1);

                var yearlySales = await context.Orders
                                               .Where(o => o.DateCreated >= startDate &&
                                                           o.DateCreated < endDate &&
                                                           o.Status == Status.Completed)
                                               .GroupBy(o => new { o.DateCreated.Year, o.DateCreated.Month })
                                               .Select(g => new { Month = g.Key.Month, Total = g.Sum(o => o.Total) })
                                               .OrderBy(g => g.Month)
                                               .Select(g => g.Total)
                                               .ToListAsync();

                return yearlySales;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }


        public async Task<decimal> GetTotalSalesBySupplierId(int supplierId)
        {
            try
            {
                var totalSales = (decimal)await context.Orders
                                          .Include(o => o.OrderItems)
                                            .ThenInclude(oi => oi.Product)
                                          .Where(o => o.Status == Status.Completed && !o.IsDeleted)
                                          .SelectMany(o => o.OrderItems)
                                          .Where(oi => oi.Product.SupplierId == supplierId)
                                          .SumAsync(oi => oi.Quantity * oi.Product.Price);

                return totalSales;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<Order> GenerateReceipt(int id)
        {
            try
            {
                var order = await context.Orders
                               .Include(a => a.Cart)
                                   .ThenInclude(c => c.Items)
                                   .ThenInclude(c => c.Product)
                               .Where(a => a.Id == id).FirstOrDefaultAsync();

                if (order == null)
                    throw new InvalidOperationException("Order not found");

                return order;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        public async Task<Order> AddOrder(OrderRequest request)
        {
            try
            {
                var existingReferenceID = await context.Orders
                                                       .Where(o => o.ReferenceId == request.ReferenceId)
                                                       .FirstOrDefaultAsync();

                if (existingReferenceID != null)
                {
                    throw new InvalidOperationException("The Reference Id you provided already exists");
                }

                var imagePath = await new ImagePathConfig().SaveProofofPayment(request.ProofOfPayment);

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

                            var sizeQuantity = await context.SizeQuantities
                                                            .Where(sq => sq.Id == cartItem.SizeQuantityId)
                                                            .FirstOrDefaultAsync();

                            if (sizeQuantity != null)
                            {
                                sizeQuantity.Quantity -= cartItem.Quantity;

                                if (sizeQuantity.Quantity < 0)
                                {
                                    throw new InvalidOperationException("Insufficient stock for the product size.");
                                }

                                context.Update(sizeQuantity);
                            }

                            context.OrderItems.Add(orderItem);
                            cartItem.IsDeleted = true;
                        }
                    }
                }

                await context.SaveChangesAsync();

                BackgroundJob.Schedule(() => UpdateOrderStatusAndNotify(order.Id, notification.Id), TimeSpan.FromSeconds(2));

                return order;
            }
            catch (Exception e)
            {
                throw new ArgumentException(e.Message);
            }
        }

        private string GenerateOrderNumber(DateTime dateCreated, int id)
        {
            return $"ORD-{dateCreated:yyMMdd}-{id:D5}";
        }

        public async Task UpdateOrderStatusAndNotify(int orderId, int id)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            UpdateOrderStatusToPending(orderId, id);
        }

        public void UpdateOrderStatusToPending(int orderId, int id)
        {
            var orderToUpdate = context.Orders.Find(orderId);
            if (orderToUpdate != null && orderToUpdate.Status == Status.OrderPlaced)
            {
                orderToUpdate.Status = Status.Pending;
                orderToUpdate.DateUpdated = DateTime.Now;
                context.SaveChangesAsync();

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
                        DateUpdated = DateTime.Now,
                        IsRead = false
                    };

                    context.Notifications.Add(notification);
                }

                context.SaveChangesAsync();
            }
        }

        public async Task<Order> ApproveOrder(int orderId)
        {
            try
            {
                var order = await context.Orders
                                         .Include(o => o.OrderItems)
                                         .Where(o => o.Id == orderId)
                                         .FirstOrDefaultAsync();

                if (order == null)
                {
                    throw new ArgumentException("Order not found");
                }

                if (order.Status != Status.Pending)
                {
                    throw new InvalidOperationException("Only orders with 'Pending' status can be approved");
                }

                order.Status = Status.Approved;
                order.DateUpdated = DateTime.Now;

                int totalQuantity = order.OrderItems.Sum(oi => oi.Quantity);
                int weekToAdd = (totalQuantity + 4) / 5;

                order.EstimatedDate = DateTime.Now.AddDays(7 * weekToAdd);

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
                        DateUpdated = DateTime.Now,
                        IsRead = false
                    };
                    context.Notifications.Add(notification);
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

        public async Task<Order> DeniedOrder(int orderId)
        {
            try
            {
                var order = await context.Orders.FindAsync(orderId);

                if (order == null)
                {
                    throw new InvalidOperationException("Order not found");
                }

                if (order.Status != Status.Pending)
                {
                    throw new InvalidOperationException("Only orders with 'Pending' status can be approved");
                }

                var orderItems = await context.OrderItems
                                              .Where(oi => oi.OrderId == orderId)
                                              .ToListAsync();

                foreach (var orderItem in orderItems)
                {
                    var sizeQuantity = await context.SizeQuantities
                                                    .Where(sq => sq.Id == orderItem.SizeQuantityId)
                                                    .FirstOrDefaultAsync();

                    if (sizeQuantity != null)
                    {
                        sizeQuantity.Quantity += orderItem.Quantity;
                        context.Update(sizeQuantity);
                    }
                }

                order.Status = Status.Denied;
                order.DateUpdated = DateTime.Now;

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
                        DateUpdated = DateTime.Now,
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

        public async Task<Order> CanceledOrder(int orderId, CancellationDto dto)
        {
            try
            {
                var order = await context.Orders.FindAsync(orderId);

                if (order == null) 
                {
                    throw new InvalidOperationException("Order not found");
                }

                if (order.Status != Status.Pending && order.Status != Status.OrderPlaced)
                {
                    throw new InvalidOperationException("Only orders with 'Pending' & 'Order Placed' status can be approved");
                }

                var orderItems = await context.OrderItems
                                              .Where(oi => oi.OrderId == orderId)
                                              .ToListAsync();

                foreach (var orderItem in orderItems)
                {
                    var sizeQuantity = await context.SizeQuantities
                                                    .Where(sq => sq.Id == orderItem.SizeQuantityId)
                                                    .FirstOrDefaultAsync();

                    if (sizeQuantity != null)
                    {
                        sizeQuantity.Quantity += orderItem.Quantity;
                        context.Update(sizeQuantity);
                    }
                }

                order.CancellationReason = dto.CancellationReason;

                order.Status = Status.Canceled;
                order.DateUpdated = DateTime.Now;

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
                        DateUpdated = DateTime.Now,
                        IsRead = false
                    };
                    await service.AddNotification(notification);
                }

                var cart = await context.Carts
                                        .Include(c => c.Items)
                                        .FirstOrDefaultAsync(c => c.Id == order.CartId);

                if (cart != null)
                {
                    var supplierNotifications = new Notification
                    {
                        UserId = cart.SupplierId,
                        OrderId = order.Id,
                        Message = $"Order {order.OrderNumber} has been canceled and requires your attention.",
                        DateCreated = DateTime.Now,
                        IsRead = false,
                        UserRole = UserRole.Supplier
                    };

                    context.Notifications.Add(supplierNotifications);
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
                    throw new InvalidOperationException("Order not found");
                }

                if (order.Status != Status.Approved)
                {
                    throw new InvalidOperationException("Only orders with 'Approved' status can be approved");
                }

                order.Status = Status.ForPickUp;
                order.DateUpdated = DateTime.Now;

                var existingNotification = await context.Notifications
                                                        .Where(o => o.OrderId == order.Id)
                                                        .FirstOrDefaultAsync();

                if (existingNotification != null)
                {
                    existingNotification.Message = $"Your order {order.OrderNumber} is ready for pick-up at the front of the UC Chapel on {order.EstimatedDate?.ToString("MM-dd-yyyy")}.";
                    existingNotification.IsRead = false;
                }
                else
                {
                    var notification = new Notification
                    {
                        UserId = order.UserId,
                        OrderId = order.Id,
                        Message = $"Your order {order.OrderNumber} is ready for pick-up at the front of the UC Chapel on {order.EstimatedDate?.ToString("MM-dd-yyyy")}.",
                        DateUpdated = DateTime.Now,
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

        public async Task SendOrderCompletedEmailAsync(string email, int orderId)
        {
            var orderDetails = await context.Orders
                                            .Where(o => o.Id == orderId && o.User.Email == email)
                                            .Include(o => o.OrderItems)
                                                .ThenInclude(oi => oi.Product)
                                            .FirstOrDefaultAsync();

            if (orderDetails == null)
            {
                throw new ArgumentException("Order not found or email does not match the order's user email.");
            }

            var itemsList = orderDetails.OrderItems.Select(oi => {
                return $@"
                    <tr>
                        <td style='padding: 10px; border-bottom: 1px solid #ddd;'>{oi.Product.ProductName}</td>
                        <td style='padding: 10px; border-bottom: 1px solid #ddd; text-align: right;'>Qty - {oi.Quantity}</td>
                    </tr>";
            }).ToList();

            string subject = "Your UNITEE Order Has Been Claimed!";
            string message = $@"
                    <html>
                    <head>
                        <style>
                            .email-body {{
                                font-family: 'Arial', sans-serif;
                                color: #333;
                                margin: 0;
                                padding: 0;
                            }}
                            .header {{
                                background-color: #4CAF50;
                                padding: 20px;
                                text-align: center;
                                font-size: 24px;
                                color: white;
                            }}
                            .order-table {{
                                width: 100%;
                                border-collapse: collapse;
                                margin-top: 20px;
                            }}
                            .order-table th {{
                                background-color: #f2f2f2;
                                padding: 10px;
                                border-bottom: 1px solid #ddd;
                            }}
                            .order-table td {{
                                padding: 10px;
                                border-bottom: 1px solid #ddd;
                            }}
                            .product-image {{
                                max-width: 100px;
                                max-height: 100px;
                            }}
                            .total-cost {{
                                text-align: right;
                                margin-top: 20px;
                                font-size: 18px;
                            }}
                            .footer {{
                                margin-top: 20px;
                                text-align: center;
                                font-size: 14px;
                                color: #999;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='email-body'>
                             <div class='header'>Order Completed</div>
                             <p class=""greeting-user-name"">Hi {orderDetails.User.FirstName} {orderDetails.User.LastName},</p>
                             <p>We are happy to inform you that your order with the reference <strong>{orderDetails.OrderNumber}</strong> has been successfully completed!</p>
                             <table class='order-table'>
                                 <tr>
                                     <th style='text-align: left;'>Product Name</th>
                                     <th style='text-align: right;'>Quantity</th>
                                 </tr>
                                  {string.Join("", itemsList)}
                             </table>
                             <div class='total-cost'><strong>Total cost:</strong> {orderDetails.Total:C}</div>
                             <p>We hope you enjoy your purchase. Feel free to reach out for any further assistance.</p>
                             <footer>
                                Thank you for shopping with UNITEE!<br>Stay stylish!</footer>
                         </div>
                    </body>
                    </html>";

            var emailConfig = new EmailConfig(configuration);
            await emailConfig.SendEmailAsync(email, subject, message);
        }

        //public async Task<Order> CompletedOrder(int orderId)
        //{
        //    try
        //    {
        //        var order = await context.Orders
        //                                  .Include(o => o.User)
        //                                  .Include(o => o.OrderItems)
        //                                     .ThenInclude(oi => oi.Product)
        //                                  .FirstOrDefaultAsync(o => o.Id == orderId);

        //        if (order == null)
        //        {
        //            throw new InvalidOperationException("Order not found");
        //        }

        //        if (order.Status != Status.ForPickUp)
        //        {
        //            throw new InvalidOperationException("Only orders with 'ForPickUp' status can be marked as completed");
        //        }

        //        order.Status = Status.Completed;
        //        order.DateUpdated = DateTime.Now;

        //        var existingNotification = await context.Notifications
        //                                                .Where(o => o.OrderId == order.Id)
        //                                                .FirstOrDefaultAsync();

        //        if (existingNotification != null)
        //        {
        //            existingNotification.Message = $"Your order {order.OrderNumber} has been completed. Thank you for shopping with us!";
        //            existingNotification.IsRead = false;
        //        }
        //        else
        //        {
        //            var notification = new Notification
        //            {
        //                UserId = order.UserId,
        //                OrderId = order.Id,
        //                Message = $"Your order {order.OrderNumber} has been completed. Thank you for shopping with us!",
        //                DateUpdated = DateTime.Now,
        //                IsRead = false
        //            };
        //            await service.AddNotification(notification);
        //        }

        //        context.Orders.Update(order);
        //        await context.SaveChangesAsync();

        //        await SendOrderCompletedEmailAsync(order.User.Email, orderId);

        //        return order;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new ArgumentException(e.Message);
        //    }
        //}

        public async Task<Order> CompletedOrder(int orderId)
        {
            try
            {
                var order = await context.Orders
                                          .Include(o => o.User)
                                          .Include(o => o.OrderItems)
                                             .ThenInclude(oi => oi.Product)
                                          .FirstOrDefaultAsync(o => o.Id == orderId);

                if (order == null)
                {
                    throw new ArgumentException("Order not found");
                }

                if (order.Status != Status.ForPickUp)
                {
                    throw new InvalidOperationException("Only orders with 'ForPickUp' status can be marked as completed");
                }

                if (order.User.IsEmailConfirmed == true)
                {
                    await SendOrderCompletedEmailAsync(order.User.Email, orderId);
                }

                order.Status = Status.Completed;
                order.DateUpdated = DateTime.Now;

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
                        DateUpdated = DateTime.Now,
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

        public async Task<Order> OrderReceived(int orderId)
        {
            try
            {
                var order = await context.Orders
                                         .Where(o => o.Id == orderId)
                                         .FirstOrDefaultAsync();

                order.IsReceived = true;

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
