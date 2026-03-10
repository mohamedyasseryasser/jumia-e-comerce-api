using e_comerce_api.DTO;
using e_comerce_api.DTO.cartdto;
using e_comerce_api.models;
using e_comerce_api.services.interfaces;
using e_comerce_api.services.reprosity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace e_comerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        public CartController(Icartreprosity icartreprosity) {
            Icartreprosity = icartreprosity;
        }
        public Icartreprosity Icartreprosity { get; }
        private string GetCurrentCustomerId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return userId;
        }
        [HttpGet("getcartbycustomer")]
        public async Task<IActionResult> getcartbycustomerid()
        {
            try
            {
                var customerid = GetCurrentCustomerId();
                var customerId = Icartreprosity.getcustomerid(customerid);

                var cart = await Icartreprosity.getcartbycustomerid(customerId);
                var response = new ApiResponse<cartdto>
                {
                    message = "Cart retrieved successfully",
                    data = cart,
                    statue = true
                };
                return Ok(response);
            }
            catch (Exception ex) {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while retrieving cart",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
            }
        [HttpGet("getcartitem/{id}")]
        public async Task<IActionResult> getcartitem(int id)
        {
            try
            {
                var customerid = GetCurrentCustomerId();
                var customerId = Icartreprosity.getcustomerid(customerid);

                var result = await Icartreprosity.CartItemExistsAndBelongsToCustomerAsync(customerid,id);
                var cartitem=await Icartreprosity.getcartitembyid(id);
                return Ok(new ApiResponse<CartItemDto>
                {
                    data = cartitem,
                    message="this cartitem",
                    statue=true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while retrieving cart item",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
        [HttpPost("addcartitems")]
        public async Task<IActionResult> addcartitems([FromBody]cartiteminputdto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Invalid coupon data",
                    ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray()
                });
            }
            try
            {
                var customerid = GetCurrentCustomerId();
                var customerId = Icartreprosity.getcustomerid(customerid);

                if (customerId==null)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Message=$"this user id:{User} is not found"
                    });
                }
                var result = await Icartreprosity.addcartitems(customerid, dto);
                return CreatedAtAction(nameof(getcartitem),
                    new {id=result.CartId},
                    new ApiResponse<CartItemDto>
                    {
                        data=result,
                        message="additems to cart successfully",
                        statue=true
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500,new ApiErrorResponse
                {
                    Message = "An error occurred while adding item to cart",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
        [HttpPut("updatecartitemquantity")]
        public async Task<IActionResult> updatcartitemquantity(int id,[FromBody]updatecartitem dto)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Invalid coupon data",
                    ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray()
                });
            }
            try
            {
                var customerid = GetCurrentCustomerId();
                if (!await Icartreprosity.CartItemExistsAndBelongsToCustomerAsync(customerid,id))
                {
                    return NotFound(new ApiResponse<object>
                    {
                        message = "Cart item not found",
                        statue = false,
                        data = null
                    });
                }
                var result =await Icartreprosity.updatecartitemquantity(customerid,dto);
                if (result==null)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Message="this cartitem is not found"
                    });
                }
                return Ok(new ApiResponse<CartItemDto>
                {
                    data=result,
                    statue=true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500,new ApiErrorResponse
                {
                    Message = "An error occurred while updating item quantity",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
        [HttpDelete("removecartitem")]
        public async Task<IActionResult> removecartitem(int cartitemid)
        {
            
            try
            {
                var currentuserid = GetCurrentCustomerId();
                if (!await Icartreprosity.CartItemExistsAndBelongsToCustomerAsync(currentuserid, cartitemid))
                {
                    return NotFound(new ApiResponse<object>
                    {
                        message = "Cart item not found",
                        statue = false,
                        data = null
                    });
                }
                var result = await Icartreprosity.removecartitem(currentuserid.ToString(),cartitemid);
                return Ok(new ApiResponse<bool>{
                    data=result,
                    message=$"remover cartitem:{cartitemid} is successfully",
                    statue = true
                });
            }
            catch(Exception ex)
            {
                return StatusCode(500,new ApiErrorResponse
                {
                    Message = "An error occurred while updating item quantity",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
        [HttpDelete("clearcart")]
        public async Task<IActionResult> clearcart()
        {
            try
            {
                var customerId = GetCurrentCustomerId();

                var result =await Icartreprosity.clearcart(customerId);

                // Invalidate cache
                //InvalidateCartCache(customerId);

                return Ok(new ApiResponse<object>
                {
                    message = "Cart cleared successfully",
                   statue = true,
                    data = null
                });
            }
            catch (Exception ex)
            {
                 return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while clearing cart",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
    }
}
