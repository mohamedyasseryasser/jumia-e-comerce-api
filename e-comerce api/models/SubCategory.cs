using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class SubCategory
    {
        [Key]
        public int SbuCatId { get; set; }
        public string Name { get; set; }
        public string description {  get; set; }
        public string image {  get; set; }
        public bool? IsActive { get; set; }
        [ForeignKey("category")]
        public int? cat_id { get; set; }
        public category? category { get; set; }
        public ICollection<ProductAttributies>? ProductAttributies { get; set; }=new List<ProductAttributies>();
        public ICollection<product>? products { get; set; } =new List<product>();
    }
}
