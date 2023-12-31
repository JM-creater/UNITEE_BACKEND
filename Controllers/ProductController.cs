using Microsoft.AspNetCore.Mvc;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;
using UNITEE_BACKEND.Services;

namespace UNITEE_BACKEND.Controllers
{
    [ApiController, Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly IProductService productService;

        public ProductController(IProductService service)
        {
            productService = service;
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
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("top-selling")]
        public ActionResult<IEnumerable<Product>> GetTopSellingProducts(int topCount = 10)
        {
            return Ok(productService.GetTopSellingProducts(topCount));
        }

        [HttpGet("top-selling-by-shop/{shopId}")]
        public ActionResult<IEnumerable<Product>> GetTopSellingProductsByShop(int shopId, int topCount = 10)
        {
            return Ok(productService.GetTopSellingProductsByShop(shopId, topCount));
        }


        [HttpGet("recommender")]
        public async Task<IActionResult> GetRecommendProducts(string search)
        {
            try
            {
                var products = await productService.RecommendProducts(search);

                return Ok(products);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("recommendations/{id}")]
        public async Task<IActionResult> GetRecommendations(int id)
        {
            try
            {
                var recommendedProducts = await productService.RecommendProductsPurchase(id);

                return Ok(recommendedProducts);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("searchByDepartment")]
        public async Task<IActionResult> GetProductsByDepartment([FromQuery] int userId)
        {
            var products = await productService.GetSearchProductByUserDepartment(userId);

            if (products == null || !products.Any())
                return NotFound("No products found for the specified user department.");

            return Ok(products);
        }

        [HttpGet]
        public IActionResult GetAllProduct()
        {
            try
            {
                var products = productService.GetAll();

                return Ok(products);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("ByShopProduct/{shopId}")]
        public IActionResult GetProductsByShopId(int shopId)
        {
            var products = productService.GetProductsByShopId(shopId);

            if (products == null || !products.Any())
            {
                return NotFound(new { Message = "No products found for this shop." });
            }

            return Ok(products);
        }

        [HttpGet("ByShop/{shopId}/ByDepartment/{departmentId}")]
        public IActionResult GetProductsByShopIdAndDepartmentId(int shopId, int departmentId)
        {
            var products = productService.GetProductsByShopIdAndDepartmentId(shopId, departmentId);

            if (products == null || !products.Any())
            {
                return NotFound(new { Message = "No products found for this shop and department." });
            }

            return Ok(products);
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetById([FromRoute] int productId)
        {
            try
            {
                var e = await productService.GetById(productId);
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
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> Update([FromRoute] int productId, [FromForm] ProductUpdateRequest request)
        {
            try
            {
                var updatedProduct = await productService.UpdateProduct(productId, request);

                return Ok("Successfully Update a Product");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("activate/{productId}")]
        public async Task<IActionResult> ActivateProduct(int productId)
        {
            try
            {
                var product = await productService.UpdateActivationStatus(productId);

                return Ok("Successfully Activated");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("deactivate/{productId}")]
        public async Task<IActionResult> DeactivateProduct(int productId)
        {
            try
            {
                var product = await productService.UpdateDectivationStatus(productId);

                return Ok("Successfully Deactivated");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
            => Ok(await productService.Delete(id));

    }
}
