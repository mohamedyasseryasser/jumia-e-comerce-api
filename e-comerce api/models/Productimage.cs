using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class ProductImage
    {
        [Key]
        public int ImageId { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }

        public string ImageUrl { get; set; }

        public int? DisplayOrder { get; set; }

        public product Product { get; set; }
    }
}
