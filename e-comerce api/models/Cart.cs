using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class Cart
    {
        [Key]
        public int Cartid {  get; set; }
        public DateTime? createdAt { get; set; }
        public decimal totalAmount {  get; set; }
        public DateTime? updatedAt { get; set; }
        [ForeignKey("customer")]
        public int? customer_id {  get; set; }
        public customer? customer { get; set; }
        public ICollection<CartItems>? CartItems { get; set; }=new List<CartItems>();
    }
}
