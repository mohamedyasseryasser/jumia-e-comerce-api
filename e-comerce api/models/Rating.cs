using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class Rating
    {
        [Key]
        public int RateId { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public int rate {  get; set; }
        public bool? IsVerifiedPurchase { get; set; }
        public int? HelpfulCount { get; set; }
        public string comment {  get; set; }
        [ForeignKey("product")]
        public int productid {  get; set; }
        [ForeignKey("customer")]
        public int customerid {  get; set; }
        public product product { get; set; }
        public customer customer { get; set; }
        public ICollection<ReviewImage> reviewImages { get; set; }=new List<ReviewImage>();
        public ICollection<HelpfulRating> helpfulRating { get; set; } =new List<HelpfulRating>();
    }
}
