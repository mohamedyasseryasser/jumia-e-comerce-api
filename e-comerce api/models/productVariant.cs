using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class productVariant
    {
        [Key]
        public int ProductVariantId { get; set; }
        public string VariantName {  get; set; } 
        public decimal basicprice {  get; set; }
        public string Description { get; set; }
        public string Mainimageurl { get; set; }
        public int stockquantity {  get; set; }
        public decimal discountpercentage { get; set; }
        public decimal FinalPrice { get; set; }
      //  public double? AverageRate { get; set; }
        public string SKU {  get; set; }
        public bool? isdefault { get; set; }
        public bool? isaveliable { get; set; }
        [ForeignKey("product")]
        public int product_id { get; set; }
        public product? product { get; set; }
       public ICollection<productVariantAttribute>? attributes { get; set; }=new List<productVariantAttribute>();
    }
}
