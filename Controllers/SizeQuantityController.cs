using Microsoft.AspNetCore.Mvc;
using UNITEE_BACKEND.Services;

namespace UNITEE_BACKEND.Controllers
{
    [ApiController, Route("[controller]")]
    public class SizeQuantityController : Controller
    {
        private ISizeQuantityService sizeQuantityService;

        public SizeQuantityController(ISizeQuantityService service)
        {
            sizeQuantityService = service;
        }


    }
}
