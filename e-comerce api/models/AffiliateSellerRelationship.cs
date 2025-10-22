using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class AffiliateSellerRelationship
    {
        [Key]
        public int RelationshipId { get; set; }
        [ForeignKey("Affiliate")]
        public int AffiliateId { get; set; }
        [ForeignKey("Seller")]
        public int SellerId { get; set; }
        public decimal? CommissionRate { get; set; }
        public string Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public  Affiliate Affiliate { get; set; }
        public  seller Seller { get; set; }
    }
}
