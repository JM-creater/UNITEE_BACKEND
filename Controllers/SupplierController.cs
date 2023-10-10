using Microsoft.AspNetCore.Mvc;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;
using UNITEE_BACKEND.Services;

namespace UNITEE_BACKEND.Controllers
{
    [ApiController, Route("[controller]")]
    public class SupplierController : Controller
    {
        private readonly ISupplierService supplierService;
        private readonly IUsersService usersService;

        public SupplierController(ISupplierService service, IUsersService services)
        {
            supplierService = service;
            usersService = services;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupplierById([FromRoute] int id)
        {
            try
            {
                var supplier = await supplierService.GetSupplierById(id);
                return Ok(supplier);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpPost("registerSupplier")]
        public async Task<IActionResult> AddSupplier([FromForm] SupplierRequest request)
        {
            try
            {
                var newSupplier = await supplierService.AddSupplier(request);
                return Ok(newSupplier);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("updateSupplier/{id}")]
        public async Task<IActionResult> UpdateSupplier(int id, [FromForm] SupplierRequest request)
        {
            try
            {
                var existingSupplier = await supplierService.GetSupplierById(id);

                if (request.Image == null && existingSupplier != null)
                {
                    request.Image = null;
                }

                var updateSupplier = await supplierService.UpdateSupplier(id, request);
                return Ok(updateSupplier);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("activate/{id}")]
        public async Task<IActionResult> Activate(int id, [FromBody] bool isActive)
        {
            try
            {
                var activateSupplier = await supplierService.ActivateSupplier(id, isActive);
                return Ok(activateSupplier);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
