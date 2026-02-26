using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace e_comerce_api.DTO.cartdto
{
    public class updatecartitem
    {
        [Required]
        public int cartitem {  get; set; }
        [Required]
        [Range(1, 100, ErrorMessage = "Quantity must be between 1 and 100")]
        public int newquantity {  get; set; }
    }
}
