using Microsoft.AspNetCore.Mvc;
using UNITEE_BACKEND.Models.Request;
using UNITEE_BACKEND.Services;

namespace UNITEE_BACKEND.Controllers
{
    [ApiController, Route("[controller]")]
    public class RatingController : ControllerBase
    {
        private readonly IRatingService service;
        public RatingController(IRatingService _service)
        {
            service = _service;
        }

        [HttpPost("rate-product")]
        public async Task<IActionResult> RatingProduct(RatingRequest request)
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
        public async Task<IActionResult> RatingSupplier(RatingRequest request)
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
    }
}
