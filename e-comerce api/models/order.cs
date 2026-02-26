using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Azure.Core.HttpHeader;

namespace e_comerce_api.models
{
    public class order
    {
        [Key]
        public int Orderid {  get; set; }
        public decimal TotalAmount {  get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
        public string statue {  get; set; }
        [ForeignKey("Address")]
         public int? address_id {  get; set; }
        [ForeignKey("customer")]
        public int? customer_id { get; set; }
        public customer? customer { get; set; }  
        public ICollection<SubOrder>? SubOrders { get; set; }=new List<SubOrder>();
        public address? Address { get; set; }
        [ForeignKey("Coupon")]
        public int? CouponId { get; set; }
        public string AffiliateCode { get; set; }
        [ForeignKey("Affiliate")]
        public int? AffiliateId { get; set; }
        public Affiliate? Affiliate { get; set; }
        public  ICollection<AffiliateComession> AffiliateCommissions { get; set; } = new List<AffiliateComession>();
        public Copons? Coupon { get; set; }
    }
}
