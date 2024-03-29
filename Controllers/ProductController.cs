﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<IEnumerable<Product>>> GetTopSellingProducts(int topCount = 10)
        {
            try
            {
                return Ok(await productService.GetTopSellingProducts(topCount));
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("top-selling-by-shop/{shopId}")]
        public async Task<ActionResult<IEnumerable<Product>>> GetTopSellingProductsByShop(int shopId, int topCount = 10)
        {
            try
            {
                return Ok(await productService.GetTopSellingProductsByShop(shopId, topCount));
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }


        [HttpGet("recommender")]
        public async Task<IActionResult> GetRecommendProducts(string search, int userId)
        {
            try
            {
                var products = await productService.RecommendProducts(search, userId);

                return Ok(products);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("recommendedForYou")]
        public async Task<IActionResult> GetRecommendProducts(int userId, int supplierId)
        {
            try
            {
                var products = await productService.GetRecommendedForYouProducts(userId, supplierId);

                return Ok(products);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("recommendedOverAll")]
        public async Task<IActionResult> GetRecommendProductsOverAll(int userId)
        {
            try
            {
                var products = await productService.GetRecommendedForYouProductsOverAll(userId);

                return Ok(products);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
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
                return BadRequest(new { message = e.Message });
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

        [HttpGet("getQuantity")]
        public async Task<IActionResult> GetQuantity(int productId, int sizeQuantityId)
        {
            try
            {
                var quantity = await productService.GetQuantityBySize(productId, sizeQuantityId);

                return Ok(quantity);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProduct()
        {
            try
            {
                var products = await productService.GetAll();

                return Ok(products);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("ByShopProduct/{shopId}")]
        public async Task<IActionResult> GetProductsByShopId(int shopId)
        {
            try
            {
                var products = await productService.GetProductsByShopId(shopId);

                if (products == null)
                {
                    return NotFound(new { Message = "No products found for this shop." });
                }

                return Ok(products);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("ByShop/{shopId}/ByDepartment/{departmentId}")]
        public async Task<IActionResult> GetProductsByShopIdAndDepartmentId(int shopId, int departmentId)
        {
            try
            {
                var products = await productService.GetProductsByShopIdAndDepartmentId(shopId, departmentId);

                if (products == null)
                {
                    return NotFound(new { Message = "No products found for this shop and department." });
                }

                return Ok(products);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetById([FromRoute] int productId)
        {
            try
            {
                var products = await productService.GetById(productId);

                return Ok(products);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
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
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> Update([FromRoute] int productId, [FromForm] ProductUpdateRequest request)
        {
            try
            {
                var updatedProduct = await productService.UpdateProduct(productId, request);

                return Ok(updatedProduct);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPut("activate/{productId}")]
        public async Task<IActionResult> ActivateProduct(int productId)
        {
            try
            {
                var product = await productService.UpdateActivationStatus(productId);

                return Ok(product);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPut("deactivate/{productId}")]
        public async Task<IActionResult> DeactivateProduct(int productId)
        {
            try
            {
                var product = await productService.UpdateDectivationStatus(productId);

                return Ok(product);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("getQuantity")]
        public async Task<IActionResult> GetQuantity(int productId, int sizeQuantityId)
        {
            try
            {
                var quantity = await productService.GetQuantityBySize(productId, sizeQuantityId);

                return Ok(quantity);
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
