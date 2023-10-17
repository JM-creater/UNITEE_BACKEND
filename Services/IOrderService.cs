using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public interface IOrderService
    {
        public Task<Order> AddOrder(OrderRequest request);
        public IEnumerable<Order> GetAll();
        public Task<Order?> GetById(int id);
        public Task<List<Order>> GetAllByUserId(int id);
        public Task<Order> GenerateReceipt(int id);
        public Task<List<Order>> GetAllBySupplierId(int supplierId);
        public Task<Order> ApproveOrder(int orderId);
        public Task<Order> DeniedOrder(int orderId);
        public Task<Order> Update(int id, Status status);
        public Task<Order> UpdateReference(UpdateReferenceRequest request);
        public Task<Order> PlaceOrder(PlaceOrderRequest request);
    }
}
