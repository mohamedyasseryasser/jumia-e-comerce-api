using System.ComponentModel.DataAnnotations;

namespace e_comerce_api.models
{
    public class Copons
    {
        [Key]
        public int CoponId {  get; set; }
        public decimal? DiscountPercentage {  get; set; }
        public string code {  get; set; }
        public string decription {  get; set; }
        public DateTime startAt { get; set; }
        public DateTime endAt { get; set; }
        public int? UsageLimit { get; set; }
        public int? UsageCount { get; set; }
        public decimal? MinimumPurchase { get; set; }
        public bool? isActive { get; set; }
        public ICollection<order> orders { get; set; }=new List<order>();
        public ICollection<UserCopon> users { get; set; }=new List<UserCopon>();
    }
}
