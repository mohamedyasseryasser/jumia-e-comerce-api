using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class SubOrder
    {
        [Key]
        public int SubOrderId {  get; set; }
        public decimal Subtotal { get; set; }
        public string Status { get; set; }
        public DateTime? StatusUpdatedAt { get; set; }
        public string TrackingNumber { get; set; }
        public string ShippingProvider { get; set; }
        [ForeignKey("seller")]
        public int? seller_id {  get; set; }
        public seller? seller { get; set; }
        [ForeignKey("order")]
        public int? OrderId { get; set; }
        public order? order { get; set; }
        public ICollection<OrderItems>? Items { get; set; }=new List<OrderItems>();
    }
}
