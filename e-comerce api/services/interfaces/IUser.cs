using e_comerce_api.DTO;
using e_comerce_api.models;

namespace e_comerce_api.services.interfaces
{
    public interface IUser
    {
        Task<Userdto> getuserbyid(useridandroledto dto);
        Task<UserStatusUpdateDto> updateuserstate(string userid, UserStatusUpdateDto dto);
        Task<UserStatusUpdateDto> deleteuser(string userid);
        Task<PagedResult<CustomerResponsedto>> GetAllCustomersAsync(int pageNumber, int pageSize);
        Task<CustomerResponsedto> getcustomerbyid(int customerid);
        Task<bool> updatecustomer(customerupdatedto dto);
        Task<UserStatusUpdateDto> deletecustomer(int customerid);
        Task<PagedResult<SellerResponseDto>> GetAllSellersAsync(int pageNumber, int pageSize);
        Task<SellerResponseDto> GetSellerByIdAsync(int sellerId);
        Task<bool> UpdateSellerAsync(SellerUpdateDto dto);
        Task<UserStatusUpdateDto> DeleteSellerAsync(int sellerId);
    //    Task<bool> SaveChangesAsync();
    }
}
