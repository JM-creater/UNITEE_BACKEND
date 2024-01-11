using Microsoft.AspNetCore.Mvc;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Models.Request;
using UNITEE_BACKEND.Models.Token;
using UNITEE_BACKEND.Services;

namespace UNITEE_BACKEND.Controllers
{
    [ApiController, Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly IUsersService service;
        private readonly AppDbContext context;
        private readonly Tokens tokens;

        public UsersController(IUsersService _service, AppDbContext dbcontext, Tokens _tokens)
        {
            service = _service;
            context = dbcontext;
            tokens = _tokens;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterRequest request)
        {
            try
            {
                var newUser = await service.RegisterCustomer(request);

                var token = tokens.GenerateJwtToken(newUser);

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
                var token = tokens.GenerateJwtToken(newSupplier);
                return Ok(new { newSupplier, Token = token });
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

                var token = tokens.GenerateJwtToken(user);

                return new JsonResult(new { user, Role = role.ToString(), Token = token });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto confirmEmailDto)
        {
            try
            {
                var user = await service.ConfirmEmail(confirmEmailDto.ConfirmationCode);

                if (user.IsEmailConfirmed)
                    return Ok(user);
                else
                    return BadRequest("Failed to confirm email.");
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPost("verify-later/{userId}")]
        public async Task<IActionResult> VerifyLater(int userId)
        {
            try
            {
                var user = await service.VerifyLater(userId);

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPost("verify-email/{userId}")]
        public async Task<IActionResult> VerifyEmail(int userId)
        {
            try
            {
                var user = await service.VerifyEmail(userId);

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
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
                return BadRequest(new { message = e.Message });
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
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("validate-reset-token")]
        public async Task<IActionResult> ValidateResetToken([FromQuery] string token)
        {
            try
            {
                var isValid = await service.IsResetTokenValid(token);

                return Ok(new { isValid });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("overAllCountUsers")]
        public IActionResult OverAllCount()
        {
            try
            {
                var user = service.OverAllCountUsers();

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("countCustomers")]
        public IActionResult CountCustomers()
        {
            try
            {
                var user = service.CountCustomers();

                return Ok(user);
            }
            catch (Exception e) 
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("countSuppliers")]
        public IActionResult CountSuppliers()
        {
            try
            {
                var user = service.CountSuppliers();

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("getTopSellingSeller")]
        public async Task<IActionResult> GetTopSellingSeller()
        {
            try
            {
                var topSeller = await service.GetTopSellingSeller();

                if (topSeller == null)
                {
                    return NotFound("No top selling seller found.");
                }

                return Ok(topSeller);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await service.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var user = await service.GetById(id);

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
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
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("getSuppliers")]
        public async Task<IActionResult> GetSuppliers()
        {
            try
            {
                var suppliers = await service.GetAllSuppliers();

                return Ok(suppliers);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
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
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("getProductsBySupplierShop/{supplierId}")]
        public async Task<IActionResult> GetProductsBySupplierShop(int supplierId)
        {
            try
            {
                var products = await service.GetProductsBySupplierShop(supplierId);

                return Ok(products);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("getCustomers")]
        public async Task<IActionResult> GetCustomers()
        {
            try
            {
                var suppliers = await service.GetAllCustomers();

                return Ok(suppliers);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("UserDepartment/{userId}")]
        public async Task<IActionResult> GetUserDepartment(int userId)
        {
            try
            {
                var user = await context.Users.FindAsync(userId);

                if (user == null)
                {
                    return NotFound(new { Message = "User not found." });
                }

                return Ok(new { departmentId = user.DepartmentId });
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("getSuppliersProduct/{departmentId}")]
        public async Task<IActionResult> GetSuppliers(int departmentId)
        {
            try
            {
                var suppliers = await service.GetAllSuppliersProducts(departmentId);

                return Ok(suppliers);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPut("updateProfileCustomer/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromForm] UpdateCustomerRequest request)
        {
            try
            {
                var user = await service.UpdateCustomerProfile(id, request);

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPut("updateProfileAdmin/{id}")]
        public async Task<IActionResult> UpdateAdmin([FromRoute] int id, [FromForm] UpdateAdminRequest request)
        {
            try
            {
                var user = await service.UpdateAdminProfile(id, request);

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPut("updateProfileSupplier/{id}")]
        public async Task<IActionResult> UpdateSupplier([FromRoute] int id, [FromForm] UpdateSupplierRequest request)
        {
            try
            {
                var user = await service.UpdateProfileSupplier(id, request);

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
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
                return BadRequest(new { message = e.Message });
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
                return BadRequest(new { message = e.Message });
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
                return BadRequest(new { message = e.Message });
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
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPut("updateAdminPassword/{id}")]
        public async Task<IActionResult> UpdateAdminPassword(int id, [FromBody] UpdatePasswordRequest request)
        {
            try
            {
                var supplier = await service.UpdateAdminPassword(id, request);

                return Ok(supplier);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }
    }
}
