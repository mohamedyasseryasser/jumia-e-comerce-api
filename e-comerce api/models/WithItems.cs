using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class WithItems
    {
        [Key]
        public int WishlistItemId { get; set; }
        [ForeignKey("Wishlist")]
        public int WishlistId { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public DateTime? AddedAt { get; set; }
        public product? Product { get; set; }
        public WishList? Wishlist { get; set; }
    }
}
