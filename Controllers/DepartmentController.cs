using Microsoft.AspNetCore.Mvc;
using UNITEE_BACKEND.Services;

namespace UNITEE_BACKEND.Controllers
{
    [ApiController, Route("[controller]")]
    public class DepartmentController : Controller
    {
        private IDepartmentService departmentService;

        public DepartmentController(IDepartmentService service)
        {
            departmentService = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(departmentService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var e = await departmentService.GetById(id);
                return Ok(e);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
