using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.DTO.cartdto
{
    public class cartiteminputdto
    {
         public int? ProductId { get; set; }
        public int Quantity { get; set; }
         public int? VariantId { get; set; }
     }
}
