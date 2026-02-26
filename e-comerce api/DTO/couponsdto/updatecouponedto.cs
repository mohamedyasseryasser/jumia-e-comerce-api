using System.ComponentModel.DataAnnotations;

namespace e_comerce_api.DTO.couponsdto
{
    public class updatecouponedto
    {
        [Required]
        public int coupounid {  get; set; }

        [StringLength(500)]
        public string Description { get; set; }

       
        [Range(0.01, 10000)]
        public decimal? DiscountAmount { get; set; }
        [Range(0, 10000)]
        public decimal? MinimumPurchase { get; set; }

        [Required]
        [RegularExpression("^(Fixed|Percentage)$", ErrorMessage = "Discount type must be either 'Fixed' or 'Percentage'")]
        public string DiscountType { get; set; }

        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public DateTime? EndDate { get; set; }

        public bool? IsActive { get; set; } = true;

        [Range(0, 10000)]
        public int? UsageLimit { get; set; }
     }
}
