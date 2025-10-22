using e_comerce_api.DTO;
using e_comerce_api.models;

namespace e_comerce_api.services.interfaces
{
    public interface IAuth
    {
       public Task<TokenResult> RegisterForAdminAsync(AdminRegisterDto dto);
       Task<TokenResult> RegisterForCustomerAsync(CustomerRegisterDto dto);
       Task<TokenResult> RegisterForSellerAsync(SellerRegisterDto dto);
       Task<TokenResult> LoginAsync(logindto logindto);
        Task AddAdmin(admin admin);
        Task AddCustomer(customer customer);
        Task AddSeller(seller seller);
        Task<roledto> addrole(roledto dto);
        Task<TokenResponseDto> RefreshTokenAsync(string refreshToken);
        Task<string> changepassword(ChangePasswordDto dto);
        Task savechanged();
    }
}
