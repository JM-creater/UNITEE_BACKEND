using Microsoft.AspNetCore.Mvc;
using UNITEE_BACKEND.Services;

namespace UNITEE_BACKEND.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;
        public OrderController(IOrderService service)
        {
            orderService = service;
        }


    }
}
