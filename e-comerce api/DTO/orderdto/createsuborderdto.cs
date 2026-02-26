using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.DTO.orderdto
{
    public class createsuborderdto
    {
         public decimal Subtotal { get; set; }
        [Required]
        public int? seller_id { get; set; }

        [Required]
        public ICollection<createorderitemsdto> Items { get; set; } = new List<createorderitemsdto>();
    }
}
