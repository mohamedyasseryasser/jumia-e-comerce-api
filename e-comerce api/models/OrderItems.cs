using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class OrderItems
    {
        [Key]
        public int OrderItemsId { get; set; }
        public int quantity {  get; set; }
        public decimal PriceAtPurchase { get; set; }
        public decimal TotalPrice { get; set; }
        [ForeignKey("Product")]
        public int? product_id {  get; set; }
        public product? Product { get; set; }
        [ForeignKey("SubOrder")]
        public int? sub_order_id {  get; set; }
        public SubOrder? SubOrder { get; set; }
        [ForeignKey("ProductVariant")]
        public int? variant_id {  get; set; }
        public productVariant? ProductVariant { get; set; }
        public virtual ICollection<AffiliateComession> AffiliateCommissions { get; set; } = new List<AffiliateComession>();

    }
}
