using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.GenerateToken;
using UNITEE_BACKEND.Models.Request;
using UNITEE_BACKEND.Services;

namespace UNITEE_BACKEND.Controllers
{
    [ApiController, Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly IUsersService service;
        private readonly AppDbContext context;
        private readonly JwtToken jwtToken;

        public UsersController(IUsersService _service, AppDbContext dbcontext, JwtToken _jwtToken)
        {
            service = _service;
            context = dbcontext;
            jwtToken = _jwtToken;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] EmailDto emailDto)
        {
            try
            {
                await service.SendEmailAsync(emailDto.Email, emailDto.Subject, emailDto.Message);

                return Ok("Email sent successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            try
            {
                var newUser = await service.RegisterCustomer(request);
                var token = jwtToken.GenerateJwtToken(newUser);
                return Ok(new { newUser, Token = token });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPost("registerSupplier")]
        public async Task<IActionResult> RegisterSupplierAccount([FromForm] SupplierRequest request)
        {
            try
            {
                var newSupplier = await service.RegisterSupplier(request);
                return Ok(newSupplier);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
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

                var (user, role) = await service.Login(request);

                if (user == null)
                {
                    return BadRequest("User not found");
                }

                var token = jwtToken.GenerateJwtToken(user);
                return new JsonResult(new { user, Role = role.ToString(), Token = token });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto confirmEmailDto)
        {
            try
            {
                var user = await service.ConfirmEmail(confirmEmailDto.ConfirmationCode);

                if (user.IsEmailConfirmed)
                    return Ok("Email confirmed successfully.");
                else
                    return BadRequest("Failed to confirm email.");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                var user = await service.ForgotPassword(email);

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            try
            {
                var user = await service.ResetPassword(dto);
                if (user != null)
                {
                    return Ok(user);
                }
                return BadRequest("Failed to reset password");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("getTopSellingSeller")]
        public async Task<IActionResult> GetTopSellingSeller()
        {
            var topSeller = await service.GetTopSellingSeller();
            if (topSeller == null)
            {
                return NotFound("No top selling seller found.");
            }

            return Ok(topSeller);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var e = await service.GetById(id);

                return Ok(e);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("getSupplierId/{id}")]
        public async Task<IActionResult> SupplierById([FromRoute] int id)
        {
            try
            {
                var supplier = await service.SupplierById(id);
                return Ok(supplier);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("getSuppliers")]
        public IActionResult GetSuppliers()
        {
            var suppliers = service.GetAllSuppliers();

            return Ok(suppliers);
        }

        [HttpGet("getSupplierById/{id}")]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            try
            {
                var supplier = await service.GetSupplierById(id);
                if (supplier == null)
                {
                    return NotFound(new { Message = "Supplier not found." });
                }

                return Ok(supplier);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("getProductsBySupplierShop/{supplierId}")]
        public IActionResult GetProductsBySupplierShop(int supplierId)
        {
            var products = service.GetProductsBySupplierShop(supplierId);

            return Ok(products);
        }

        [HttpGet("getCustomers")]
        public IActionResult GetCustomers()
        {
            var suppliers = service.GetAllCustomers();

            return Ok(suppliers);
        }

        [HttpGet("UserDepartment/{userId}")]
        public IActionResult GetUserDepartment(int userId)
        {
            var user = context.Users.Find(userId);

            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            return Ok(new { departmentId = user.DepartmentId });
        }

        [HttpGet("getSuppliersProduct/{departmentId}")]
        public IActionResult GetSuppliers(int departmentId)
        {
            var suppliers = service.GetAllSuppliersProducts(departmentId);

            return Ok(suppliers);
        }

        [HttpPut("updateProfileCustomer/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCustomerRequest request)
        {
            try
            {
                var user = await service.UpdateCustomerProfile(id, request);
                return Ok("Successfully Updated");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("updateProfileSupplier/{id}")]
        public async Task<IActionResult> UpdateSupplier([FromRoute] int id, [FromBody] UpdateSupplierRequest request)
        {
            try
            {
                var user = await service.UpdateProfileSupplier(id, request);
                return Ok("Successfully Updated");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPut("validateCustomer/{id}")]
        public async Task<IActionResult> ValidateCustomerAccount([FromRoute] int id, [FromBody] ValidateUserRequest request)
        {
            try
            {
                var user = await service.ValidateCustomer(id, request);
                return Ok(user);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPut("validateSupplier/{id}")]
        public async Task<IActionResult> ValidateSupplierAccount([FromRoute] int id, [FromBody] ValidateUserRequest request)
        {
            try
            {
                var user = await service.ValidateSupplier(id, request);
                return Ok(user);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPut("updateCustomerPassword/{id}")]
        public async Task<IActionResult> UpdateCustomerPassword(int id, [FromBody] UpdatePasswordRequest request)
        {
            try
            {
                var supplier = await service.UpdateCustomerPassword(id, request);

                return Ok(supplier);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("updateSupplierPassword/{id}")]
        public async Task<IActionResult> UpdateSupplierPassword(int id, [FromBody] UpdatePasswordRequest request)
        {
            try
            {
                var supplier = await service.UpdateSupplierPassword(id, request);

                return Ok(supplier);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
