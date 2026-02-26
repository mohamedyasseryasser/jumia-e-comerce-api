using e_comerce_api.models;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.DTO.orderdto
{
    public class orderitemsdto
    {
        public int OrderItemsId { get; set; }
        public int quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public decimal TotalPrice { get; set; }
         public int product_id { get; set; }
        public int sub_order_id { get; set; }
    
        public int? variant_id { get; set; }
     }
}
