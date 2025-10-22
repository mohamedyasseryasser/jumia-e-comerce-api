using System.ComponentModel.DataAnnotations;

namespace e_comerce_api.models
{
    public class category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public string images {  get; set; }
        public ICollection<SubCategory>? subCategories { get; set; }
    }
}
