using e_comerce_api.services.reprosity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class customer
    {
        [Key]
        public int Customerid {  get; set; }
        [ForeignKey("user")]
        public string? user_id{ get; set; }
        public applicationuser? user { get; set; }
        //collection of orders
        public ICollection<order>? orders { get; set; }=new List<order>();
        //cart
        public  ICollection<Cart> Carts { get; set; } = new List<Cart>();
        public  ICollection<HelpfulRating> HelpfulRatings { get; set; } = new List<HelpfulRating>();
        public  ICollection<productview> ProductViews { get; set; } = new List<productview>();
        public  ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<UserCopon> UserCoupons { get; set; } = new List<UserCopon>();
        public  ICollection<WishList> Wishlists { get; set; } = new List<WishList>();

    }
}
