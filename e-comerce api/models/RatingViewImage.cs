using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class ReviewImage
    {
        public int ReviewImageId { get; set; }
        [ForeignKey("Rating")]
        public int RatingId { get; set; }
        public string ImageUrl { get; set; }
        public  Rating Rating { get; set; }
    }
}
