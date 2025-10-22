using e_comerce_api.DTO;
using e_comerce_api.models;
using e_comerce_api.services.interfaces;
using e_comerce_api.services.reprosity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace e_comerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public AccountController(IAuth authrepo,UserManager<applicationuser> usermanager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            Authrepo = authrepo;
            Usermanager = usermanager;
            RoleManager = roleManager;

            Configuration = configuration;
        }

        public IAuth Authrepo { get; }
        public UserManager<applicationuser> Usermanager { get; }
        public RoleManager<IdentityRole> RoleManager { get; }

        public IConfiguration Configuration { get; }
      [Authorize(Roles ="admin")]
        [HttpPost("addrole")]
        //add role
        public async Task<IActionResult> addrole(roledto role)
        {
            try
            {
                
                var result = await Authrepo.addrole(role);
                if (result == null) {
                    throw new Exception("role is null");
                   }
                return Ok(result);
            }
            catch (Exception ex) 
            {
                throw new Exception($"error is: {ex}");
            }
        }
       // [Authorize(Roles ="admin")]
        [HttpPost("addadmin")]
        //register for admin
        public async Task<IActionResult> RegisterForAdmin(AdminRegisterDto dto)
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

            var existingUser = await Usermanager.FindByEmailAsync(dto.email);
            if (existingUser != null)
                return BadRequest("This email already exists");

            try
            {
                var tokenResult = await Authrepo.RegisterForAdminAsync(dto);
                return Ok(tokenResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        //register for seller
        [HttpPost("addseller")]
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

            var existingUser = await Usermanager.FindByEmailAsync(dto.email);
            if (existingUser != null)
                return BadRequest("This email already exists");

            try
            {
                var tokenResult = await Authrepo.RegisterForSellerAsync(dto);
                return Ok(tokenResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        //register for customer
        [HttpPost("addcustomer")]
        public async Task<IActionResult> registerforcustomer(CustomerRegisterDto dto)
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

            var existingUser = await Usermanager.FindByEmailAsync(dto.email);
            if (existingUser != null)
                return BadRequest("This email already exists");

            try
            {
                var tokenResult = await Authrepo.RegisterForCustomerAsync(dto);
                return Ok(tokenResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        // login function 
        [HttpPost("login")]
        public async Task<IActionResult> login(logindto dto)
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
            try
            {
                var tokenResult = await Authrepo.LoginAsync(dto);
                return Ok(tokenResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                return BadRequest("Refresh token is required.");

            try
            {
                var tokenResponse = await Authrepo.RefreshTokenAsync(request.RefreshToken);
                return Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
        //change password
        [HttpPut("change-password")]
        public async Task<IActionResult> changepassword(ChangePasswordDto dto)
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
            try
            {
                var result = await Authrepo.changepassword(dto);
                if (result==null||string.IsNullOrEmpty(result))
                {
                    return BadRequest(ModelState);
                }
                return Ok($"the change password is successfully");
            }catch(Exception ex)
            {
                return BadRequest(new {message=ex.Message});
            }
        }
    }

}
   
