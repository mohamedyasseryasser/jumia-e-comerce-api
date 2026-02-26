using e_comerce_api.DTO;
using e_comerce_api.DTO.cartdto;
using e_comerce_api.models;
using e_comerce_api.services.interfaces;
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
        private int GetCurrentCustomerId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("Unable to determine current user");
            }

            return userId;
        }
        [HttpGet("getcartbycustomer")]
        public async Task<IActionResult> getcartbycustomerid()
        {
            try
            {
                var customerid = GetCurrentCustomerId();
                var cart = await Icartreprosity.getcartbycustomerid(customerid);
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
                int customerid = GetCurrentCustomerId();
                var result = await Icartreprosity.CartItemExistsAndBelongsToCustomerAsync(customerid.ToString(),id);
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

    }
}
