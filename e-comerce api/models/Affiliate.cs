using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class Affiliate
    {
        [Key]
        public int AffiliateId { get; set; }
        [ForeignKey("user")]
        public string UserId { get; set; }
        public string AffiliateCode { get; set; }
        public decimal? CommissionRate { get; set; }
        public decimal? TotalEarnings { get; set; }
        public decimal? AvailableBalance { get; set; }
        public decimal? WithdrawnAmount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsActive { get; set; }
        public applicationuser user { get; set; }
        public ICollection<AffiliateSellerRelationship> affiliateSellerRelationships { get; set; }=new List<AffiliateSellerRelationship>();
        public ICollection<order> orders { get; set; }=new List<order>();
        public ICollection<AffiliateComession> affiliateComessions { get; set; }=new List<AffiliateComession>();
        public ICollection<AffiliateWithDrawel> affiliateWithdrawel { get; set; }= new List<AffiliateWithDrawel>();
    }
}
