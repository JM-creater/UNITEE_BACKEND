﻿using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using UNITEE_BACKEND.DatabaseContext;
using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;
using UNITEE_BACKEND.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

namespace UNITEE_BACKEND.Controllers
{
    [ApiController, Route("[controller]")]
    public class UsersController : Controller
    {
        private IUsersService usersService;
        private readonly AppDbContext context;

        public UsersController(IUsersService service, AppDbContext dbcontext)
        {
            usersService = service;
            context = dbcontext;
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
                return BadRequest(new { message = e.Message });
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

        [HttpGet("getSupplierById/{id}")]
        public async Task<IActionResult> GetSupplierById(int id)
        {
            try
            {
                var supplier = usersService.GetSupplierById(id);
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
            var products = usersService.GetProductsBySupplierShop(supplierId);
            return Ok(products);
        }


        [HttpGet("getCustomers")]
        public IActionResult GetCustomers()
        {
            var suppliers = usersService.GetAllCustomers();
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
            var suppliers = usersService.GetAllSuppliersProducts(departmentId);
            return Ok(suppliers);
        }

        [HttpPut("updateCustomer/{id}")]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCustomerRequest request)
        {
            try
            {
                var user = await usersService.Update(id, request);
                return Ok("Successfully Updated");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("updateSupplier/{id}")]
        public async Task<IActionResult> UpdateSupplier([FromRoute] int id, [FromBody] UpdateSupplierRequest request)
        {
            try
            {
                var user = await usersService.UpdateSupplier(id, request);
                return Ok("Successfully Updated");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPut("validateCustomer/{id}")]
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

        [HttpPut("validateSupplier/{id}")]
        public async Task<IActionResult> ValidateSupplierAccount([FromRoute] int id, [FromBody] ValidateUserRequest request)
        {
            try
            {
                var user = await usersService.ValidateSupplier(id, request);
                return Ok(user);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPut("updateCustomerPassword/{id}")]
        public async Task<IActionResult> UpdateSupplierPassword(int id, [FromBody] UpdatePasswordRequest request)
        {
            try
            {
                var user = await usersService.UpdatePassword(id, request);

                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
