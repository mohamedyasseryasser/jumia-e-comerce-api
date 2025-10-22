using Azure.Core;
using e_comerce_api.Controllers;
using e_comerce_api.DTO;
using e_comerce_api.models;
using e_comerce_api.models;
using e_comerce_api.services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace e_comerce_api.services.reprosity
{
    public class Auth:IAuth
    {
        public RoleManager<IdentityRole> RoleManager { get; }
        public context Context { get; }
        public UserManager<applicationuser> Usermanager { get; }
        public IConfiguration Configuration { get; }

        public Auth(RoleManager<IdentityRole> roleManager,context context,UserManager<applicationuser> userManager, IConfiguration configuration)
        {
            RoleManager = roleManager;
            Context = context;
            Usermanager = userManager;
            Configuration = configuration;
        }
        public async Task<TokenResult> RegisterForAdminAsync(AdminRegisterDto dto)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                var user = new applicationuser
                {
                    UserName = dto.username,
                    Email = dto.email,
                    createdAt = DateTime.Now,
                    updatedAt = DateTime.Now,
                    IsActive = true,
                    lastlogin = DateTime.Now,
                    PhoneNumber = dto.phone,
                };
                var result = await Usermanager.CreateAsync(user, dto.password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"فشل إنشاء المستخدم: {errors}");
                }
                var newaddress = new address
                {
                    AddressName = dto.address.AddressName,
                    State = dto.address.State,
                    IsDefault = dto.address.IsDefault,
                    StreetAddress = dto.address.StreetAddress,
                    City = dto.address.City,
                    Country = dto.address.Country,
                    PostalCode = dto.address.PostalCode,
                    Orders = new List<order>(),
                }; 
                user.addresses.Add(newaddress);
                var roleResult = await Usermanager.AddToRoleAsync(user, "admin");
                if (roleResult.Succeeded)
                {
                    var newadmin = new admin { 
                       user_id=user.Id,
                       role="admin",
                       permission=dto.permission
                    };
                    await AddAdmin(newadmin);
                }
                else
                {
                    throw new Exception("Admin role not found");
                }
                var token = await CreateToken(user);
               
                await savechanged();

                await transaction.CommitAsync();

                return token;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

       public async Task<TokenResult> RegisterForCustomerAsync(CustomerRegisterDto dto)
        {
            using var transaction =await Context.Database.BeginTransactionAsync();
            try
            {
                var user = new applicationuser
                {
                    UserName = dto.username,
                    Email = dto.email,
                    createdAt = DateTime.Now,
                    updatedAt = DateTime.Now,
                    IsActive = true,
                    lastlogin = DateTime.Now,
                    PhoneNumber = dto.phone,
                };
                var result = await Usermanager.CreateAsync(user, dto.password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"فشل إنشاء المستخدم: {errors}");
                }
                var newaddress = new address
                {
                    AddressName = dto.address.AddressName,
                    State = dto.address.State,
                    IsDefault = dto.address.IsDefault,
                    StreetAddress = dto.address.StreetAddress,
                    City = dto.address.City,
                    Country = dto.address.Country,
                    PostalCode = dto.address.PostalCode,
                    Orders = new List<order>(),
                };
                user.addresses.Add(newaddress);
                var roleResult = await Usermanager.AddToRoleAsync(user, "customer");
                if (roleResult.Succeeded)
                {
                    var newcustomer = new customer
                    {
                        user_id = user.Id,
                        orders = new List<order>()
                    };
                    await AddCustomer(newcustomer);
                }
                else
                {
                    throw new Exception("Customer role not found");
                }
                var token = await CreateToken(user);

                await savechanged();

                await transaction.CommitAsync();

                return token;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
      public  async Task<TokenResult> RegisterForSellerAsync(SellerRegisterDto dto)
        {
            using var transaction =await Context.Database.BeginTransactionAsync();
            try
            {
                var user = new applicationuser
                {
                    UserName = dto.username,
                    Email = dto.email,
                    createdAt = DateTime.Now,
                    updatedAt = DateTime.Now,
                    IsActive = true,
                    lastlogin = DateTime.Now,
                    PhoneNumber = dto.phone,
                };
                var result = await Usermanager.CreateAsync(user, dto.password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"فشل إنشاء المستخدم: {errors}");
                }
                var newaddress = new address
                {
                    AddressName = dto.address.AddressName,
                    State = dto.address.State,
                    IsDefault = dto.address.IsDefault,
                    StreetAddress = dto.address.StreetAddress,
                    City = dto.address.City,
                    Country = dto.address.Country,
                    PostalCode = dto.address.PostalCode,
                    Orders = new List<order>(),
                };
                user.addresses.Add(newaddress);
                var roleResult = await Usermanager.AddToRoleAsync(user, "seller");
                if (roleResult.Succeeded)
                {
                    var newseller = new seller
                    {
                        user_id = user.Id,
                        businessDesription = dto.businessDesription,
                       businessName = dto.businessName,
                       CustomerServicesPhone=dto.CustomerServicesPhone,
                       is_verfied = dto.is_verfied,
                       logo=dto.logo,
                       rate=dto.rate,
                       verfiedAt=dto.verfiedAt,
                    };
                    await AddSeller(newseller);
                }
                else
                {
                    throw new Exception("Customer role not found");
                }
                var token = await CreateToken(user);

                await savechanged();

                await transaction.CommitAsync();

                return token;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
       public  async Task<TokenResult> LoginAsync(logindto dto)
        {
            try
            {
                var currentuser = await Usermanager.FindByNameAsync(dto.username);
                if (currentuser != null)
                {
                    if (currentuser.IsActive==false)
                    {
                        throw new Exception($"{dto.username} is not active");
                    }
                    var found = await Usermanager.CheckPasswordAsync(currentuser, dto.password);
                    if (found != true)
                    {
                        throw new Exception($"username and password is invalid");
                    }
                }
                return await CreateToken(currentuser);
            }
            catch (Exception ex) 
            {
              throw new Exception($"username and password is invalid");
            }
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private async Task<TokenResult> CreateToken(applicationuser currentuser)
        {
            SecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:secret"]));
            //create token
            SigningCredentials signingCredentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, currentuser.UserName));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, currentuser.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            var roles =await Usermanager.GetRolesAsync(currentuser);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            JwtSecurityToken tokenreprestion = new JwtSecurityToken(
                issuer: Configuration["JWT:validissuer"],
                audience: Configuration["JWT:validaudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: signingCredentials
            ); string tokenString = new JwtSecurityTokenHandler().WriteToken(tokenreprestion);
            var refreshToken = GenerateRefreshToken();
            currentuser.RefreshToken = refreshToken;
            currentuser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // مثلاً 7 أيام صلاحية
            await Usermanager.UpdateAsync(currentuser);

            return new TokenResult
            {
                Token = tokenString,
                ExpireAt = tokenreprestion.ValidTo,
                RefreshToken = refreshToken,
                RefreshTokenExpireAt = currentuser.RefreshTokenExpiryTime.Value
            };
        }
        // tracking
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await Context.Database.BeginTransactionAsync();
        }
        // add admin
        public async Task AddAdmin(admin admin)
        {
            await Context.Admins.AddAsync(admin);
        }
        // add customer
        public async Task AddCustomer(customer customer)
        {
            await Context.Customers.AddAsync(customer);
        }
        //add seller
        public async Task AddSeller(seller seller)
        {
            await Context.Sellers.AddAsync(seller);
        }
        //add role
        public async Task<roledto> addrole(roledto dto)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();

            try
            {
                if (string.IsNullOrEmpty(dto.role))
                    throw new Exception("this role is not founded");


                var roleExist = await RoleManager.RoleExistsAsync(dto.role);
                if (roleExist)
                    throw new Exception("Role already exists");

                var result = await RoleManager.CreateAsync(new IdentityRole(dto.role));
 
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"فشل إنشاء المستخدم: {errors}");
                }
                await transaction.CommitAsync();
                return new roledto {role=dto.role };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception  ($"Error is :{ex}");
            }
        }
        //refresh token
        public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken)
        {
            var user = Context.Users.FirstOrDefault(u => u.RefreshToken == refreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new Exception("Invalid or expired refresh token.");

            // إنشاء Access Token جديد
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(Configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(5), // صلاحية قصيرة
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var newToken = tokenHandler.CreateToken(tokenDescriptor);

            return new TokenResponseDto
            {
                AccessToken = tokenHandler.WriteToken(newToken),
                RefreshToken = user.RefreshToken
            };
        }
        //change password
        public async Task<string> changepassword(ChangePasswordDto dto)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                var exitemail = await Usermanager.FindByEmailAsync(dto.email);
                if (exitemail == null)
                {
                    throw new Exception("this email is not found");
                }
                var check = await Usermanager.CheckPasswordAsync(exitemail, dto.oldpassword);
                if (!check)
                {
                    throw new Exception("this password is not correct");
                }
                var result = await Usermanager.ChangePasswordAsync(exitemail, dto.oldpassword, dto.newpassword);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to change password: {errors}");
                }
                exitemail.PasswordHash = dto.newpassword;
                await transaction.CommitAsync();
                return exitemail.PasswordHash;
            }
            catch (Exception ex) {
                await transaction.RollbackAsync();
                throw new Exception($"Error is {ex}");
            }
         }
        public async Task savechanged()
        {
            await Context.SaveChangesAsync();
        }
    }
}
