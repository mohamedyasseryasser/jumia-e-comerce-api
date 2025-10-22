namespace e_comerce_api.DTO
{
    public class SubCategoryDto
    {
        public string Name { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public bool? IsActive { get; set; }
        public int? cat_id { get; set; }
    }

    public class SubCategoryResponse
    {
        public int SubCatId { get; set; }
        public string Name { get; set; }
        public string description { get; set; }
        public string image { get; set; }
        public bool? IsActive { get; set; }
        public int? cat_id { get; set; }
    }
}
