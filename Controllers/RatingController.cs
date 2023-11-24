using Microsoft.AspNetCore.Mvc;
using UNITEE_BACKEND.Models.Request;
using UNITEE_BACKEND.Services;

namespace UNITEE_BACKEND.Controllers
{
    [ApiController, Route("[controller]")]
    public class RatingController : Controller
    {
        private readonly IRatingService service;
        public RatingController(IRatingService _service)
        {
            service = _service;
        }

        [HttpPost("rate-product")]
        public async Task<IActionResult> RatingProduct([FromBody] RatingRequest request)
        {
            try
            {
                var rating = await service.SubmitRatingProduct(request);

                return Ok(rating);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("rate-supplier")]
        public async Task<IActionResult> RatingSupplier([FromBody] RatingRequest request)
        {
            try
            {
                var rating = await service.SubmitRatingSupplier(request);

                return Ok(rating);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("average-product-rating/{productId}")]
        public async Task<IActionResult> GetAverageProductRating(int productId)
        {
            try
            {
                var averageRating = await service.GetAverageProductRating(productId);
                return Ok(new { AverageRating = averageRating });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("average-supplier-rating/{supplierId}")]
        public async Task<IActionResult> GetAverageSupplierRating(int supplierId)
        {
            try
            {
                var averageRating = await service.GetAverageSupplierRating(supplierId);
                return Ok(new { AverageRating = averageRating });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet("{userId}")]
        public async Task<IActionResult> GetRatingByUser(int userId)
        {
            try
            {
                var rating = await service.GetRatingByUser(userId);

                return Ok(rating);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
