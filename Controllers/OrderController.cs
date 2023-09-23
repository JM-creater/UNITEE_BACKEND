using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> AddOrder(OrderRequest request)
        {
            try
            {
                var order = await orderService.AddOrder(request);

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateReference([FromForm] UpdateReferenceRequest request)
        {
            try
            {
                var order = await orderService.UpdateReference(request);

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByUserId(int id)
        {
            try
            {
                var order = await orderService.GetAllByUserId(id);

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

        [HttpPut("place")]
        public async Task<IActionResult> UpdateOrderPlace([FromBody] PlaceOrderRequest request)
        {
            try
            {
                var order = await orderService.PlaceOrder(request);

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] OrderUpdateRequest request)
        {
            try
            {
                var order = await orderService.Update(request.Id, request.Status);

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
