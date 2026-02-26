using System.Text.Json.Serialization;

namespace e_comerce_api.DTO
{
    public class updateproductinputdto
    {
        public int productid {  get; set; }
        public string Name { get; set; }
        public int subcategoryid { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public int StockQuantity { get; set; }
        public int SubcategoryId { get; set; }
        public int SellerId { get; set; }
        public string ProductVariantsJson {  get; set; }
        public string attributesvaluesjson {  get; set; }
        public IFormFile MainImageFile { get; set; }
        public List<IFormFile> AdditionalImageFiles { get; set; }
        public bool HasVariants { get; set; }
        [JsonIgnore]
        public List<CreateProductAttributeValueDto> AttributeValues { get; set; } = new List<CreateProductAttributeValueDto>();
        [JsonIgnore]
        public List<UpdateProductBaseVariantDto> Variants { get; set; } = new List<UpdateProductBaseVariantDto>();
        public List<int> productimagedeleted { get; set; }
    }
    public class UpdateProductBaseVariantDto
    {
      
        public string Description { get; set; }

        public int? VariantId { get; set; }
        public string VariantName { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public int StockQuantity { get; set; }
        public string Sku { get; set; }
        public bool IsDefault { get; set; }
         public IFormFile VariantImage { get; set; }
       // public string VariantImage { get; set; }

        public List<ProductVariantAttributeDto> VariantAttributes { get; set; }
    }
    
}
