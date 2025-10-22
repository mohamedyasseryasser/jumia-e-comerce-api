using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public string Mainimageurl {  get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public decimal basicprice {  get; set; }
        public decimal discountpercentage { get; set; }
        public decimal FinalPrice { get; set; }
        public int stockquantity {  get; set; }
        public double AverageRate {  get; set; }
        public bool isaveliable {  get; set; }
        [ForeignKey("seller")]
        public int? seller_id { get; set; }
        public seller? seller { get; set; }
        [ForeignKey("subcategory")]
        public int? sub_cat_id {  get; set; }
        public SubCategory? subcategory { get; set; }
         public ICollection<ProductImage> ProductImage { get; set; }= new List<ProductImage>();
        public ICollection<productVariant>? productVariants { get; set; }= new List<productVariant>();
        public ICollection<Rating> ratings { get; set; }=new List<Rating>();
        public  ICollection<AffiliateComession> AffiliateCommissions { get; set; } = new List<AffiliateComession>();
        public  ICollection<CartItems> CartItems { get; set; } = new List<CartItems>();
        public  ICollection<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
        public  ICollection<ProductAttributiesValue> ProductAttributeValues { get; set; } = new List<ProductAttributiesValue>();
         public  ICollection<productview> ProductViews { get; set; } = new List<productview>();
         public  ICollection<WithItems> WishlistItems { get; set; } = new List<WithItems>();
    }
}

