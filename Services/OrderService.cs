using Hangfire;
using Microsoft.EntityFrameworkCore;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Request;


namespace UNITEE_BACKEND.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext context;
        private readonly INotificationService service;

        public OrderService(AppDbContext dbcontext, INotificationService notificationService)
        {
            context = dbcontext;
            this.service = notificationService;

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
                            .Include(u => u.Cart)
                                .ThenInclude(s => s.Supplier)
                            .Include(u => u.Cart)
                                .ThenInclude(i => i.Items)
                                .ThenInclude(p => p.Product)
                                .ThenInclude(s => s.Sizes)
                            .Where(o => o.UserId == id).ToListAsync();

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

        //public async Task<Order> AddOrder(OrderRequest request)
        //{
        //    try
        //    {
        //        var imagePath = await ProofofPayment(request.ProofOfPayment);

        //        int nextId;
        //        if (context.Orders.Any())
        //        {
        //            nextId = context.Orders.Max(o => o.Id) + 1;
        //        }
        //        else
        //        {
        //            nextId = 1;
        //        }

        //        var order = new Order
        //        {
        //            UserId = request.UserId,
        //            CartId = request.CartId,
        //            Total = request.Total,
        //            ProofOfPayment = imagePath,
        //            ReferenceId = request.ReferenceId,
        //            DateCreated = DateTime.Now,
        //            DateUpdated = DateTime.Now,
        //            PaymentType = PaymentType.EMoney,
        //            Status = Status.Pending,
        //            OrderNumber = GenerateOrderNumber(DateTime.Now, nextId)
        //        };

        //        context.Orders.Add(order);
        //        await context.SaveChangesAsync();

        //        return order;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new InvalidOperationException(e.Message);
        //    }
        //}

        //public async Task<Order> AddOrder(OrderRequest request)
        //{
        //    try
        //    {
        //        var imagePath = await ProofofPayment(request.ProofOfPayment);

        //        int nextId;
        //        if (context.Orders.Any())
        //        {
        //            nextId = context.Orders.Max(o => o.Id) + 1;
        //        }
        //        else
        //        {
        //            nextId = 1;
        //        }

        //        var order = new Order
        //        {
        //            UserId = request.UserId,
        //            CartId = request.CartId,
        //            Total = request.Total,
        //            ProofOfPayment = imagePath,
        //            ReferenceId = request.ReferenceId,
        //            DateCreated = DateTime.Now,
        //            DateUpdated = DateTime.Now,
        //            PaymentType = PaymentType.EMoney,
        //            Status = Status.Pending,
        //            OrderNumber = GenerateOrderNumber(DateTime.Now, nextId)
        //        };

        //        context.Orders.Add(order);
        //        await context.SaveChangesAsync();

        //        // Retrieve the cart associated with the order
        //        var cart = await context.Carts
        //                                .Include(c => c.Items)
        //                                .FirstOrDefaultAsync(c => c.Id == request.CartId && !c.IsDeleted);

        //        if (cart != null)
        //        {
        //            // Assuming the OrderRequest contains details about ordered items
        //            if (request.OrderItems.Count() == cart.Items.Count())
        //            {
        //                // All items in the cart are ordered, so mark the cart as deleted
        //                cart.IsDeleted = true;
        //                await context.SaveChangesAsync();
        //            }
        //        }

        //        return order;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new InvalidOperationException(e.Message);
        //    }
        //}

        public async Task<Order> AddOrder(OrderRequest request)
        {
            try
            {
                var imagePath = await ProofofPayment(request.ProofOfPayment);

                int nextId;
                if (context.Orders.Any())
                {
                    nextId = context.Orders.Max(o => o.Id) + 1;
                }
                else
                {
                    nextId = 1;
                }

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
                    Status = Status.Pending,
                    OrderNumber = GenerateOrderNumber(DateTime.Now, nextId)
                };

                context.Orders.Add(order);
                await context.SaveChangesAsync();

                var cart = await context.Carts
                                        .Include(c => c.Items)
                                        .FirstOrDefaultAsync(c => c.Id == request.CartId && !c.IsDeleted);

                if (cart != null)
                {
                    var orderedProductIds = request.OrderItems.Select(oi => oi.ProductId).ToList();

                    var cartProductIds = cart.Items.Select(ci => ci.ProductId).ToList();

                    if (!cartProductIds.Except(orderedProductIds).Any())
                    {
                        cart.IsDeleted = true;
                        await context.SaveChangesAsync();
                    }
                }

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
                var order = await context.Orders.FindAsync(orderId);

                if (order == null)
                {
                    throw new ArgumentException("Order not found");
                }

                if (order.Status != Status.Pending)
                {
                    throw new InvalidOperationException("Only orders with pending status can be approved");
                }

                order.Status = Status.Approved;
                
                var notification = new Notification
                {
                    UserId = order.UserId,
                    OrderId = order.Id,
                    Message = $"Your order {order.OrderNumber} has been approved!"
                };

                await service.AddNotification(notification);

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
                    throw new InvalidOperationException("Only orders with pending status can be approved");
                }

                order.Status = Status.Denied;

                var notification = new Notification
                {
                    UserId = order.UserId,
                    OrderId = order.Id,
                    Message = $"Your order {order.OrderNumber} has been denied!"
                };

                await service.AddNotification(notification);

                context.Orders.Update(order);
                await context.SaveChangesAsync();

                return order;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Order> UpdateReference(UpdateReferenceRequest request)
        {
            try
            {
                var order = await GetById(request.Id);

                if (order == null)
                    throw new Exception("Order not found");

                //var imagePath = await SaveImage(request.ProofOfPayment);

                order.ReferenceId = request.ReferenceId;
                //order.ProofOfPayment = request.ProofOfPayment;
                order.DateUpdated = DateTime.Now;
                order.Status = Status.Pending;

                context.Orders.Update(order);

                await context.SaveChangesAsync();

                return order;

            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<Order> PlaceOrder(PlaceOrderRequest request)
        {
            try
            {
                var order = await GetById(request.Id);

                if (order == null)
                    throw new Exception("Order not found");

                order.Status = Status.Pending;
                order.DateUpdated = DateTime.Now;
                order.EstimateDate = request.EstimateDate;

                context.Orders.Update(order);

                await context.SaveChangesAsync();

                return order;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Order> Update(int id, Status status)
        {
            try
            {
                var order = await GetById(id);

                if (order == null)
                    throw new Exception("Order not found");

                order.Status = status;
                order.DateUpdated = DateTime.Now;

                context.Orders.Update(order);

                await context.SaveChangesAsync();

                return order;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

    }
}
