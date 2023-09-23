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

        public OrderService(AppDbContext dbcontext)
        {
            context = dbcontext;
        }

        public async Task<Order?> GetById(int id)
            => await context.Orders.FirstOrDefaultAsync(o => o.Id == id);

        public async Task<List<Order>> GetAllByUserId(int id)
            => await context.Orders.Where(o => o.UserId == id).ToListAsync();

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
                var order = context.Orders.Add(new Order { 
                    UserId = request.UserId,
                    CartId = request.CartId,
                    Total = request.Total,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now,
                    PaymentType = PaymentType.EMoney,
                    Status = Status.Pending
                });

                if (order == null)
                    throw new Exception("Order not found");

                await context.SaveChangesAsync();
                return order.Entity;
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

                var imagePath = await SaveImage(request.ProofOfPayment);

                order.ReferenceId = request.ReferenceId;
                order.ProofOfPayment = imagePath;
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

                order.Status = Status.OrderPlaced;
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

        public async Task<string?> SaveImage(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            string folder = Path.Combine(Directory.GetCurrentDirectory(), "Reference");
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

            return Path.Combine("Reference", fileName);
        }



    }
}
