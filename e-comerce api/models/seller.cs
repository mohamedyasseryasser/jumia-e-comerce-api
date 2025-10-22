using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class seller
    {
        [Key]
        public int Sellerid { get; set; }
        [ForeignKey("user")]
        public string? user_id { get; set; }
        public applicationuser? user { get; set; }
        [Required]
        public string businessName {  get; set; }
        [Required]
        public string CustomerServicesPhone {  get; set; }
        [Required]
        public string businessDesription {  get; set; }
        [Required]
        public string logo {  get; set; }
        public float rate {  get; set; }
        [Required]
        public bool is_verfied {  get; set; }
        public DateTime verfiedAt { get; set; }
        public virtual ICollection<AffiliateComession> AffiliateCommissions { get; set; } = new List<AffiliateComession>();

        public virtual ICollection<AffiliateSellerRelationship> AffiliateSellerRelationships { get; set; } = new List<AffiliateSellerRelationship>();

        public virtual ICollection<product> Products { get; set; } = new List<product>();

        public virtual ICollection<SubOrder> SubOrders { get; set; } = new List<SubOrder>();
    }
}
