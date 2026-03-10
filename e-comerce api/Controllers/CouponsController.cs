using e_comerce_api.DTO;
using e_comerce_api.DTO.couponsdto;
using e_comerce_api.services.interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace e_comerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponsController : ControllerBase
    {
        public CouponsController(ICopounereprosity copounereprosity)
        {
            Copounereprosity = copounereprosity;
        }
        public ICopounereprosity Copounereprosity { get; }
        [HttpGet("getcouponebyid")]
        public async Task<IActionResult> getcouponebyid(int id)
        {
            try
            {

                var result =await Copounereprosity.GetCouponByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        message = "Coupon not found",
                        statue = false,
                        data = null
                    });
                }

                var response = new ApiResponse<couponresponsedto>
                {
                    message = "Coupon retrieved successfully",
                    data = result,
                    statue = true
                };
                return Ok(response);
            }
            catch (Exception ex) {
                return StatusCode(500,new ApiErrorResponse
                {
                    Message = $"An error occurred while retrieving coupon with id = {id}",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
        [HttpPost("createcoupon")]
        public async Task<IActionResult> createcoupone([FromBody]createcouponinputdto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponse
                {
                    ErrorMessages = ModelState.Values.
                    SelectMany(x => x.Errors).
                    Select(x => x.ErrorMessage).ToArray(),
                    Message = "this data is not valid"
                });
            }
            try
            {
                if (await Copounereprosity.codecouponeexit(dto.Code))
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "Coupon creation failed",
                        ErrorMessages = new string[] { $"Coupon code '{dto.Code}' already exists" }
                    });
                }
                var result = await Copounereprosity.createcopoune(dto);
                return CreatedAtAction(nameof(getcouponebyid),
                    new {id=result.CoponId},
                    new ApiResponse<couponresponsedto>
                    {
                        data = result,
                        statue=true,
                        message="created coupone successfully"
                    });
            }
            catch (Exception ex)
            {
                return StatusCode(500,new ApiErrorResponse
                {
                    Message = "An error occurred while creating coupon",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
        [HttpPut("updatecoupoune")]
        public async Task<IActionResult> updatecoupone(int id,[FromBody]updatecouponedto dto)
        {
            if (id!=dto.coupounid)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message=$"this id{id} is not equal id in body{dto.coupounid}"
                });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "this data is not valid",
                    ErrorMessages = ModelState.Values.
                    SelectMany(v => v.Errors).
                    Select(x => x.ErrorMessage).ToArray()
                });
            }
            try
            {
                var coupone = await Copounereprosity.updatecoupone(dto);
                if (coupone==null)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Message="this copone is not found" 
                    });
                }
                return Ok(new ApiResponse<couponresponsedto>
                {
                    data = coupone,
                    statue = true,
                    message = "done"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500,new ApiErrorResponse
                {
                    Message = "An error occurred while updating coupon",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
        [HttpDelete("deletcoupone")]
        public async Task<IActionResult> deletecoupone(int id)
        {
            try
            {
                var exitcoupone=await Copounereprosity.GetCouponByIdAsync(id);
                if (exitcoupone==null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        data =null,
                        message="this coupone is not found",
                        statue=false
                    }); 
                }
                var result = await Copounereprosity.deletecoupone(id);
                return Ok(new ApiResponse<bool>
                {
                    data = result,
                    message="done",
                    statue=true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while deleting the coupon",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
    }
}
