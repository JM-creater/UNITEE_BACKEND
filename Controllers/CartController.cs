using Microsoft.AspNetCore.Mvc;
using UNITEE_BACKEND.Enum;
using UNITEE_BACKEND.Models.Request;
using UNITEE_BACKEND.Services;

namespace UNITEE_BACKEND.Controllers
{
    [ApiController, Route("[controller]")]
    public class CartController : Controller
    {
        private ICartService cartService;

        public CartController(ICartService service)
        {
            cartService = service;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(cartService.GetAll());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var e = await cartService.GetById(id);
                return Ok(e);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet("bycustomer/{customerId}")]
        public async Task<IActionResult> GetCartByCustomer([FromRoute] int customerId)
        {
            try
            {
                var customer = await cartService.GetCartByCustomer(customerId);
                return Ok(customer);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToCart([FromBody] CartAddRequest request)
        {
            try
            {
                var userRole = UserRole.Customer;

                await cartService.AddToCart(request.UserId, request.ProductId, request.Quantity, userRole);
                return Ok("Item added to cart");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] CartAddRequest request)
        {
            try
            {
                var existingCart = await cartService.GetById(id);

                var updateCart = await cartService.Update(id, request);

                return Ok(updateCart);
            }
            catch (Exception e)
            {

                return BadRequest(e);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
            => Ok(await cartService.Delete(id));

        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> RemoveFromCart([FromRoute] int id)
        {
            try
            {
                await cartService.RemoveRestoreStock(id);
                return Ok("Item removed from cart");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
