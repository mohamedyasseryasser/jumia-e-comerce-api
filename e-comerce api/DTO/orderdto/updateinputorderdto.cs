using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.DTO.orderdto
{
    public class updateinputorderdto
    {

        [Required]
        public int OrderId { get; set; }
        public int? CouponId { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? ShippingFee { get; set; }
        public decimal? TaxAmount { get; set; }
        [Range(0.01, double.MaxValue, ErrorMessage = "Final amount must be greater than 0")]
        public decimal? FinalAmount { get; set; }
        [StringLength(10)]
        public string PaymentStatus { get; set; }
        public string OrderStatus { get; set; }
    }
}
