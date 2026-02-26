using e_comerce_api.Enum;

namespace e_comerce_api.DTO.cartdto
{
    public class cartdto
    {
        public int Cartid { get; set; }
        public cartstatue statue { get; set; } = cartstatue.Open;
        public DateTime? createdAt { get; set; }
        public decimal totalAmount { get; set; }
        public DateTime? updatedAt { get; set; }
        public int? customerid {  get; set; }
         public ICollection<CartItemDto> cartItemDtos { get; set; }=new List<CartItemDto>();
    }
}
