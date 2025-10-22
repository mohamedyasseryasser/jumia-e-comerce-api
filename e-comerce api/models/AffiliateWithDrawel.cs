using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class AffiliateWithDrawel
    {
        [Key]
        public int WithdrawalId { get; set; }
        [ForeignKey("Affiliate")]
        public int AffiliateId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; }

        public string PaymentDetails { get; set; }

        public string Status { get; set; }

        public DateTime? RequestedAt { get; set; }

        public DateTime? ProcessedAt { get; set; }

        public string Notes { get; set; }

        public  Affiliate Affiliate { get; set; }
    }
}
