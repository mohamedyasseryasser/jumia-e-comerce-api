using e_comerce_api.models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.DTO.orderdto
{
    public class createorderdto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int AddressId { get; set; }

        public int? CouponId { get; set; }

        [Required]
        [StringLength(20)]
        public string PaymentMethod { get; set; }

        public int? AffiliateId { get; set; }

        [StringLength(20)]
        public string AffiliateCode { get; set; }

     public List<createorderitemsdto> Items { get; set; }=new List<createorderitemsdto>();
 
        // These properties will be calculated by the backend
        public decimal TotalAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal FinalAmount { get; set; }

        // This will be populated by the backend
        public List<createsuborderdto> SubOrders { get; set; }=new List<createsuborderdto>();
    }
}
