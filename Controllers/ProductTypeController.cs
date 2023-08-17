using Microsoft.AspNetCore.Mvc;
using UNITEE_BACKEND.Services;

namespace UNITEE_BACKEND.Controllers
{
    [ApiController, Route("[controller]")]
    public class ProductTypeController : Controller
    {
        private IProductTypeService productTypeService;

        public ProductTypeController(IProductTypeService service) 
        {
            productTypeService = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(productTypeService.GetAll());
        }
    }
}
