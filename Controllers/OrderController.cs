using Microsoft.AspNetCore.Mvc;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;
using UNITEE_BACKEND.Services;

namespace UNITEE_BACKEND.Controllers
{
    [ApiController, Route("[controller]")]
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;
        public OrderController(IOrderService service)
        {
            orderService = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddOrder([FromForm] OrderRequest request)
        {
            try
            {
                var order = await orderService.AddOrder(request);

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAllOrder()
        {
            try
            {
                var order = orderService.GetAll();

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            try
            {
                var order = await orderService.GetAllByUserId(userId);

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("receipt/{id}")]
        public async Task<IActionResult> GenerateReceipt(int id)
        {
            try
            {
                var order = await orderService.GenerateReceipt(id);

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("BySupplier/{supplierId}")]
        public async Task<ActionResult<List<Order>>> GetBySupplier(int supplierId)
        {
            return await orderService.GetAllBySupplierId(supplierId);
        }

        [HttpPut("approvedOrder/{orderId}")]
        public async Task<IActionResult> ApprovedOrders(int orderId)
        {
            try
            {
                var order = await orderService.ApproveOrder(orderId);

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("deniedOrder/{orderId}")]
        public async Task<IActionResult> DeniedOrders(int orderId)
        {
            try
            {
                var order = await orderService.DeniedOrder(orderId);

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("orderCanceled/{orderId}")]
        public async Task<IActionResult> OrderCanceled(int orderId)
        {
            try
            {
                var order = await orderService.CanceledOrder(orderId);

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("forPickUp/{orderId}")]
        public async Task<IActionResult> ForPickUpOrders(int orderId)
        {
            try
            {
                var order = await orderService.ForPickUp(orderId);

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("orderCompleted/{orderId}")]
        public async Task<IActionResult> OrderCompleted(int orderId)
        {
            try
            {
                var order = await orderService.CompletedOrder(orderId);

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
