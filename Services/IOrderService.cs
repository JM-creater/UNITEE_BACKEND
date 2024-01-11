using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.ImageDirectory;
using UNITEE_BACKEND.Models.Request;

namespace UNITEE_BACKEND.Services
{
    public interface IOrderService
    {
        public Task<Order> AddOrder(OrderRequest request);
        public int CountPendingOrders(int id);
        public int CountApprovedOrders(int id);
        public int CountForPickUpOrders(int id);
        public int CountCompletedOrderd(int id);
        public int CountCanceledOrderd(int id);
        public Task<IEnumerable<Order>> GetAll();
        public Task<Order?> GetById(int id);
        public Task<List<Order>> GetAllByUserId(int id);
        public Task<decimal> GetTotalSalesBySupplierId(int supplierId);
        public Task<IEnumerable<float>> GetWeeklySales(DateTime startDate, int supplierId);
        public Task<IEnumerable<float>> GetMonthlySales(int year, int month, int supplierId);
        public Task<IEnumerable<float>> GetYearlySales(int year, int supplierId);
        public Task<IEnumerable<float>> GetWeeklySalesAdmin(DateTime startDate);
        public Task<IEnumerable<float>> GetMonthlySalesAdmin(int year, int month);
        public Task<IEnumerable<float>> GetYearlySalesAdmin(int year);
        public Task<Order> GenerateReceipt(int id);
        public Task<List<Order>> GetAllBySupplierId(int supplierId);
        public Task<Order> ApproveOrder(int orderId);
        public Task<Order> DeniedOrder(int orderId);
        public Task<Order> CanceledOrder(int orderId, CancellationDto dto);
        public Task<Order> ForPickUp(int orderId);
        public Task<Order> CompletedOrder(int orderId);
        public Task<Order> OrderReceived(int orderId);
    }
}
