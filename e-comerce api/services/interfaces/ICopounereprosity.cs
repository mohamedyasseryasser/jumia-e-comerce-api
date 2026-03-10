using e_comerce_api.DTO.couponsdto;

namespace e_comerce_api.services.interfaces
{
    public interface ICopounereprosity
    {
        public Task<couponresponsedto> createcopoune(createcouponinputdto dto);
        public Task<bool> codecouponeexit(string code);
        public Task<couponresponsedto> GetCouponByIdAsync(int couponId);
        public Task<couponresponsedto> updatecoupone(updatecouponedto couponDto);
        public Task<bool> deletecoupone(int id);

    }
}
