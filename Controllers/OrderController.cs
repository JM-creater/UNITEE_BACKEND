﻿using Microsoft.AspNetCore.Mvc;
using UNITEE_BACKEND.Dto;
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

        [HttpGet("countPending/{id}")]
        public IActionResult CountPendingOrders(int id)
        {
            try
            {
                int count = orderService.CountPendingOrders(id);

                return Ok(count);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("countApproved/{id}")]
        public IActionResult CountApprovedOrders(int id)
        {
            try
            {
                int count = orderService.CountApprovedOrders(id);

                return Ok(count);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("countForPickUp/{id}")]
        public IActionResult CountForPickUpOrders(int id)
        {
            try
            {
                int count = orderService.CountForPickUpOrders(id);

                return Ok(count);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("countCompleted/{id}")]
        public IActionResult CountCompletedOrders(int id)
        {
            try
            {
                int count = orderService.CountCompletedOrderd(id);

                return Ok(count);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("countCanceled/{id}")]
        public IActionResult CountCanceledOrders(int id)
        {
            try
            {
                int count = orderService.CountCanceledOrderd(id);

                return Ok(count);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOrder()
        {
            try
            {
                var order = await orderService.GetAll();

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
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
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("weekly")]
        public async Task<IActionResult> GetWeeklySales([FromQuery] DateTime startDate, [FromQuery] int supplierId)
        {
            try
            {
                var result = await orderService.GetWeeklySales(startDate, supplierId);

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("monthly")]
        public async Task<IActionResult> GetMonthlySales([FromQuery] int year, [FromQuery] int month, [FromQuery] int supplierId)
        {
            try
            {
                var result = await orderService.GetMonthlySales(year, month, supplierId);

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("yearly")]
        public async Task<IActionResult> GetYearlySales([FromQuery] int year, [FromQuery] int supplierId)
        {
            try
            {
                var result = await orderService.GetYearlySales(year, supplierId);

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("weeklyAdmin")]
        public async Task<IActionResult> GetWeeklySalesAdmin([FromQuery] DateTime startDate)
        {
            try
            {
                var result = await orderService.GetWeeklySalesAdmin(startDate);

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("monthlyAdmin")]
        public async Task<IActionResult> GetMonthlySalesAdmin([FromQuery] int year, [FromQuery] int month)
        {
            try
            {
                var result = await orderService.GetMonthlySalesAdmin(year, month);

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("yearlyAdmin")]
        public async Task<IActionResult> GetYearlySalesAdmin([FromQuery] int year)
        {
            try
            {
                var result = await orderService.GetYearlySalesAdmin(year);

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("totalSales/{supplierId}")]
        public async Task<IActionResult> GetTotalSales(int supplierId)
        {
            try
            {
                var totalSales = await orderService.GetTotalSalesBySupplierId(supplierId);

                return Ok(totalSales);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
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
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("BySupplier/{supplierId}")]
        public async Task<ActionResult<List<Order>>> GetBySupplier(int supplierId)
        {
            try
            {
                var order = await orderService.GetAllBySupplierId(supplierId);

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
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
                return BadRequest(new { message = e.Message });
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
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPut("orderCanceled/{orderId}")]
        public async Task<IActionResult> OrderCanceled(int orderId, [FromBody] CancellationDto dto)
        {
            try
            {
                var order = await orderService.CanceledOrder(orderId, dto);

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
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
                return BadRequest(new { message = e.Message });
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
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPut("orderReceived/{orderId}")]
        public async Task<IActionResult> OrderReceived(int orderId)
        {
            try
            {
                var order = await orderService.OrderReceived(orderId);

                return Ok(order);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
