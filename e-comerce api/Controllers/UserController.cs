
using e_comerce_api.DTO;
using e_comerce_api.models;
using e_comerce_api.services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Threading.Tasks;

namespace e_comerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUser _userRepo;
        private readonly IAuth authrepo;
        private readonly UserManager<applicationuser> _userManager;

        public UserController(IUser userrepo, IAuth _authrepo, UserManager<applicationuser> userManager)
        {
            _userRepo = userrepo;
            authrepo = _authrepo;
            _userManager = userManager;
        }

        private string? GetCurrentUser()
        {
            if (User?.Identity?.IsAuthenticated != true)
                return null;

            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [Authorize]
        [EnableRateLimiting("standard")]
        [ResponseCache(Duration = 60)]
        [HttpGet("getcurrentuserdetails")]
        public async Task<IActionResult> GetCurrentUserDetails()
        {
            var userId = GetCurrentUser();
            if (userId == null)
                return Unauthorized("Please login");

            var role = User.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrWhiteSpace(role))
                return NotFound("User has no role");

            try
            {
                var userData = await _userRepo.getuserbyid(new useridandroledto { role = role, UserId = userId });
                return Ok(userData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet("{userid}/getuserbyidforadmin")]
        public async Task<IActionResult> GetUserByIdForAdmin(string userid)
        {
            var result = await _userManager.FindByIdAsync(userid);
            if (result == null)
                return NotFound("User not found");

            var role = User.FindFirstValue(ClaimTypes.Role);
            if (string.IsNullOrWhiteSpace(role))
                return NotFound("User has no role");

            try
            {
                var userData = await _userRepo.getuserbyid(new useridandroledto { role = role, UserId = userid });
                return Ok(userData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        private bool IsAdmin()
        {
            return User.IsInRole("admin");
        }

        [Authorize]
        [HttpPut("updateuserstatue")]
        public async Task<IActionResult> UpdateUserStatus(string id, [FromBody] UserStatusUpdateDto dto)
        {
            try
            {
                if (!IsAdmin())
                    return Forbid("You are not an Admin ❌");

                var result = await _userRepo.updateuserstate(id, dto);
                if (result.IsActive != null)
                    return Ok($"this new isactive is : {result.IsActive}");

                return BadRequest(new ApiErrorResponse
                {
                    Message = "Invalid data",
                    ErrorMessages = ModelState.Values
          .SelectMany(v => v.Errors)
          .Select(e => e.ErrorMessage)
          .ToArray()
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpDelete("deleteuser")]
        public async Task<IActionResult> deleteuser(string userid)
        {
            try
            {
                if (!IsAdmin())
                    return Forbid("You are not an Admin ❌");
                var result = await _userRepo.deleteuser(userid);
                if (result == null)
                    return NotFound(new ApiErrorResponse
                    {
                        Message = "Invalid data",
                        ErrorMessages = ModelState.Values
         .SelectMany(v => v.Errors)
         .Select(e => e.ErrorMessage)
         .ToArray()
                    });
                return Ok($"deleted is successfully and isactive become : {result.IsActive}");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // customers CRUD
        //get all customers pagination
        [HttpGet("customer-pagination")]
        [Authorize]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            if (!IsAdmin())
                return Forbid("You are not an Admin ❌");
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("Page number and page size must be greater than zero.");

            var result = await _userRepo.GetAllCustomersAsync(pageNumber, pageSize);
            return Ok(result);
        }
        // get customer by id
        [Authorize]
        [HttpGet("customerbyid")]
        public async Task<IActionResult> getcustomerbyid(int customerid)
        {
            try
            {
               
                var exitcustomer = await _userRepo.getcustomerbyid(customerid);
                if (exitcustomer == null)
                    return NotFound(new ApiErrorResponse
                    {
                        Message = "Invalid data",
                        ErrorMessages = ModelState.Values
         .SelectMany(v => v.Errors)
         .Select(e => e.ErrorMessage)
         .ToArray()
                    });
                if (!IsAdmin() && GetCurrentUser() != exitcustomer.user_id)
                    return Forbid("You are not an Admin ❌");
                return Ok(exitcustomer);
            }
            catch (Exception ex)
            {
                throw new Exception($"error is {ex.Message}");
            }
        }
        //register customer 
        [HttpPost("register-new-customer")]
        public async Task<IActionResult> registercustomer([FromBody] CustomerRegisterDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "Invalid data",
                        ErrorMessages = ModelState.Values
         .SelectMany(v => v.Errors)
         .Select(e => e.ErrorMessage)
         .ToArray()
                    });
                }
                var customer = await authrepo.RegisterForCustomerAsync(dto);
                if (customer == null)
                    return BadRequest("register customer is failed");
                return Ok(customer);
            }
            catch (Exception ex) { throw; }
        }
        //update customer
        //[Authorize]
        [HttpPut("update-customer")]
        public async Task<IActionResult> updatecustomer(customerupdatedto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "Invalid data",
                        ErrorMessages = ModelState.Values
         .SelectMany(v => v.Errors)
         .Select(e => e.ErrorMessage)
         .ToArray()
                    });
                if (GetCurrentUser()!=dto.user_id)
                    return Unauthorized();
                var result = await _userRepo.updatecustomer(dto);
                if (result != true)
                    return BadRequest("update customer is failed");
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //delete customer
        [Authorize]
        [HttpDelete("delete-customer")]
        public async Task<IActionResult> deletecustomer(int customerid)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "Invalid data",
                        ErrorMessages = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToArray()
                    });
                }
                if (!IsAdmin())
                    return Forbid("You are not an Admin ❌");
                var result = await _userRepo.deletecustomer(customerid);
                if (result == null)
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "Invalid data",
                        ErrorMessages = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToArray()
                    });
                return Ok(result);
            }
            catch
            {
                throw;
            }
        }
        //crud seller
       
        [HttpGet("pagination")]
        public async Task<IActionResult> GetAllSellers(int pageNumber = 1, int pageSize = 10)
        {
            if (!IsAdmin())
                return Forbid("Admins only ❌");

            var result = await _userRepo.GetAllSellersAsync(pageNumber, pageSize);
            return Ok(result);
        }
        [HttpPost("register-seller")]
        public async Task<IActionResult> registerforseller(SellerRegisterDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Invalid data",
                    ErrorMessages = ModelState.Values
         .SelectMany(v => v.Errors)
         .Select(e => e.ErrorMessage)
         .ToArray()
                });
            
            var existingUser = await _userManager.FindByEmailAsync(dto.email);
            if (existingUser != null)
                return BadRequest("This email already exists");
            if (GetCurrentUser() != existingUser.Id && !IsAdmin())
                return Forbid("You are not authorized ❌");
            try
            {
                var tokenResult = await authrepo.RegisterForSellerAsync(dto);
                return Ok(tokenResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [Authorize]
        [HttpGet("{sellerId}")]
        public async Task<IActionResult> GetSellerById(int sellerId)
        {
            var seller = await _userRepo.GetSellerByIdAsync(sellerId);
            if (seller == null)
                return NotFound("Seller not found");

            if (!IsAdmin() && GetCurrentUser() != seller.user_id)
                return Forbid("You are not authorized ❌");

            return Ok(seller);
        }

        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateSeller(SellerUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Invalid data",
                    ErrorMessages = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToArray()
                });

            if (GetCurrentUser() != dto.user_id && !IsAdmin())
                return Forbid("You are not authorized ❌");

            var result = await _userRepo.UpdateSellerAsync(dto);
            return result ? Ok("Seller updated successfully") : BadRequest("Failed to update seller");
        }

        [Authorize]
        [HttpDelete("delete/{sellerId}")]
        public async Task<IActionResult> DeleteSeller(int sellerId)
        {
            if (!IsAdmin())
                return Forbid("Admins only ❌");

            var result = await _userRepo.DeleteSellerAsync(sellerId);
            return Ok(result);
        }
    }
}
