using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class HelpfulRating
    {
        [Key]
        public int HelpfulId { get; set; }

        [ForeignKey("Rating")]
        public int RatingId { get; set; }
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public bool IsHelpful { get; set; }
        public customer Customer { get; set; }
        public  Rating Rating { get; set; }
    }
}
