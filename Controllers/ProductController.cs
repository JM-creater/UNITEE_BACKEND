using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;
using UNITEE_BACKEND.Services;

namespace UNITEE_BACKEND.Controllers
{
    [ApiController, Route("[controller]")]
    public class ProductController : Controller
    {
        private IProductService productService;

        public ProductController(IProductService service)
        {
            productService = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(productService.GetAll());
        }

        [HttpGet("byid/{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var e = await productService.GetById(id);
                return Ok(e);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("bysupplier/{supplierId}")]
        public async Task<IActionResult> GetProductsBySupplier([FromRoute] int supplierId)
        {
            try
            {
                var products = await productService.GetProductsBySupplier(supplierId);
                return Ok(products);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("addproduct")]
        public async Task<IActionResult> AddProduct([FromForm] ProductRequest request)
        {
            try
            {
                var newProduct = await productService.AddProduct(request);
                return Ok(newProduct);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductRequest request)
        {
            try
            {
                var existingProduct = await productService.GetById(id);

                if (request.Image == null && existingProduct != null)
                {
                    request.Image = null;
                }

                var updatedProduct = await productService.Update(id, request);
                return Ok(updatedProduct);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("activate/{productId}")]
        public async Task<IActionResult> ActivateProduct(int productId, [FromBody] bool isActive)
        {
            try
            {
                var product = await productService.UpdateActivationStatus(productId, isActive);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
            => Ok(await productService.Delete(id));

    }
}
