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

        [HttpPost("add")]
        public async Task<IActionResult> AddToCartUser([FromBody] CartAddRequest request)
        {
            try
            {
                var userRole = UserRole.Customer;
                await cartService.AddToCart(userRole, request);

                return Ok("Item added to cart");
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
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
            catch (Exception e)
            {
                return BadRequest(e.Message);
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
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("myCart/{userId}")]
        public async Task<IActionResult> GetMyCartData([FromRoute] int userId)
        {
            try
            {
                var myCartItems = await cartService.GetByUserId(userId);
                return Ok(myCartItems);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
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

        [HttpDelete("deleteCart/{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            try
            {
                await cartService.DeleteCart(id);

                return Ok(new { Message = $"Cart with ID {id} has been soft-deleted." });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                var deletedCart = await cartService.Delete(id);
                return Ok(deletedCart);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

    }
}
