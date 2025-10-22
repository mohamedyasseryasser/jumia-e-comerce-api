using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class CartItems
    {
        [Key]
        public int CartItemsId { get; set; }
        [ForeignKey("Cart")]
        public int? CartId { get; set; }
        [ForeignKey("Product")]
        public int? ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtAddition { get; set; }
        [ForeignKey("Variant")]
        public int? VariantId { get; set; }
        public Cart? Cart { get; set; }
        public  product? Product { get; set; }
        public productVariant? Variant { get; set; }
    }
}
