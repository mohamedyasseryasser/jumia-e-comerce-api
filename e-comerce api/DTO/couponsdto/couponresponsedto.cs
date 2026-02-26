using e_comerce_api.DTO.orderdto;

namespace e_comerce_api.DTO.couponsdto
{
    public class couponresponsedto
    {
        public int CoponId { get; set; }
        public bool? isActive { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string code { get; set; }
        public string decription { get; set; }
        public DateTime startAt { get; set; }
        public DateTime endAt { get; set; }
        public string DiscountType { get; set; }
        public int? UsageLimit { get; set; }
        public int? UsageCount { get; set; }
        public decimal? MinimumPurchase { get; set; }
        public int usercount {  get; set; }
        public int ordercount {  get; set; }
        public ICollection<usercouponinputdto> UserCoupons { get; set; } = new List<usercouponinputdto>();
        public ICollection<ordercouponresponsedto> orderresponsedtos { get; set; }= new List<ordercouponresponsedto>();
    }
}
