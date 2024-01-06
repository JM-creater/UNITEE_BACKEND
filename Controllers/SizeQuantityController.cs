using Microsoft.AspNetCore.Mvc;
using UNITEE_BACKEND.Dto;
using UNITEE_BACKEND.Entities;
using UNITEE_BACKEND.Models.Request;
using UNITEE_BACKEND.Services;

namespace UNITEE_BACKEND.Controllers
{
    [ApiController, Route("[controller]")]
    public class SizeQuantityController : Controller
    {
        private readonly ISizeQuantityService service;

        public SizeQuantityController(ISizeQuantityService _service)
        {
            service = _service;
        }

        [HttpPost("createsizequantity")]
        public async Task<IActionResult> CreateSizeQuantity(SizeRequest request)
        {
            try
            {
                var sizeQuantity = await service.AddSizeQuantity(request);

                return Ok(sizeQuantity);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSizeQuantity([FromBody] CreateSizeQuantityDto dto)
        {
            try
            {
                var sizeQuantity = await service.Create(dto);

                return CreatedAtAction(nameof(AddSizeQuantity), new { id = sizeQuantity.Id }, sizeQuantity);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> AddSizeToProduct(int productId, [FromBody] SizeQuantityDto dto)
        {
            try
            {
                var sizeQuantity = await service.AddSizeToProduct(productId, dto);

                return CreatedAtAction(nameof(AddSizeToProduct), new { id = sizeQuantity.Id }, sizeQuantity);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                return Ok(await service.GetAll());
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("byproduct/{id}")]
        public async Task<IActionResult> GetByProductId([FromRoute] int id)
        {
            try
            {
                var sizeQuantities = await service.GetByProductId(id);

                return Ok(sizeQuantities);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("ForProduct/{productId}")]
        public async Task<IActionResult> GetSizeQuantityProduct(int productId)
        {
            try
            {
                var sizeQuantities = await service.GetSizeQuantitiesForProduct(productId);

                return Ok(sizeQuantities);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateSizeQuantity([FromRoute] int id, [FromForm] UpdateSizeQuantityDto dto)
        {
            try
            {
                var sizequantity = await service.Update(id, dto);

                return Ok("Successfully Updated the Size and Quantity");
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpGet("{productId}/sizes")]
        public async Task<IActionResult> GetProductSizes(int productId)
        {
            try
            {
                var sizes = await service.GetSizesByProductId(productId);
                return Ok(sizes);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpPut("UpdateSizes/{productId}/{size}")]
        public async Task<IActionResult> UpdateSizeQuantity(int id, int productId, string size, int newQuantity)
        {
            try
            {
                var sizeQuantity = await service.UpdateQuantity(id, productId, size, newQuantity);

                return Ok(sizeQuantity);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var result = await service.DeleteSizeQuantity(id);

                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(new { message = e.Message });
            }
        }

    }
}
