using Microsoft.AspNetCore.Mvc;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;
using UNITEE_BACKEND.Services;

namespace UNITEE_BACKEND.Controllers
{
    [ApiController, Route("[controller]")]
    public class UsersController : Controller
    {
        private IUsersService usersService;

        public UsersController(IUsersService service)
        {
            usersService = service;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!request.Id.HasValue && string.IsNullOrWhiteSpace(request.Email)) 
                {
                    return BadRequest("Provide either user ID or email for login");
                }

                var (user, role) = await usersService.Login(request);
                return new JsonResult(new { user, role = role.ToString() });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            try
            {
                var newUser = await usersService.Register(request);
                return Ok(newUser);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(usersService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var e = await usersService.GetById(id);
                return Ok(e);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("getSuppliers")]
        public IActionResult GetSuppliers()
        {
            var suppliers = usersService.GetAllSuppliers();
            return Ok(suppliers);
        }

        [HttpGet("getCustomers")]
        public IActionResult GetCustomers()
        {
            var suppliers = usersService.GetAllCustomers();
            return Ok(suppliers);
        }

        [HttpPut]
        public async Task<IActionResult> Update(User request)
        {
            try
            {
                var e = await usersService.Update(request);
                return Ok(e);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("validate/{id}")]
        public async Task<IActionResult> ValidateUserAccount([FromRoute] int id, [FromBody] ValidateUserRequest request)
        {
            try
            {
                var user = await usersService.ValidateUser(id, request);
                return Ok(user);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
            => Ok(await usersService.Delete(id));
    }
}
