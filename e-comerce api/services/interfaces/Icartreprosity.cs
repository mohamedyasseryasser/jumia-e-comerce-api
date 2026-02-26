using e_comerce_api.DTO.cartdto;
using e_comerce_api.Enum;
using e_comerce_api.models;

namespace e_comerce_api.services.interfaces
{
    public interface Icartreprosity
    {
        Task<cartdto> getcartbycustomerid(int customerid, bool isincludeitems = true);
        
        Task<CartItemDto> getcartitembyid(int cartitemid);
       
        //add cartitems
        Task<CartItemDto> addcartitems(string userid, cartiteminputdto dto);
        
        //update cartitem quantity 
        Task<CartItemDto> updatecartitemquantity(string userid, updatecartitem dto);
         
        //remove cartitem
        Task<bool> removecartitem(string userid, int cartitemid);

        //clear cart
       Task<bool> clearcart(string userid);

        //product exit in cart or no
        Task<bool> productexitincart(string userid, int productid, int? variantid = null);
        
        Task<bool> CartItemExistsAndBelongsToCustomerAsync(string userId, int cartItemId);
      
         Task<int> GetCartItemsCountAsync(int customerId);
      
    }
}
