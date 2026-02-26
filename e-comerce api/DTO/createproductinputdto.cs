using e_comerce_api.models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace e_comerce_api.DTO
{
    public class CreateProductInputDto
    {
        public string Name { get; set; }
        public string subcategory { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public int StockQuantity { get; set; }
        public int SubcategoryId { get; set; }
        public int SellerId { get; set; }
        public string attributesvaluesjson { get; set; } 
        public string ProductVariantsJson{  get; set; }
        public IFormFile MainImageFile { get; set; }
        public List<IFormFile> AdditionalImageFiles { get; set; }
        public bool HasVariants { get; set; }

        [JsonIgnore]
        // **التعديل هنا:** قم بتهيئة القائمة لتجنب مشكلة التحقق من الصحة
        public List<CreateProductAttributeValueDto> AttributeValues { get; set; } = new List<CreateProductAttributeValueDto>();

        [JsonIgnore]
        // **التعديل هنا:** قم بتهيئة القائمة لتجنب مشكلة التحقق من الصحة
        public List<CreateProductBaseVariantDto> Variants { get; set; } = new List<CreateProductBaseVariantDto>();
       // [JsonIgnore]
         // public List<CreateProductAttributeValueDto> AttributeValues { get; set; }
          //[JsonIgnore]
          //public List<CreateProductBaseVariantDto> Variants { get; set; }
    }
    public class ProductImageInputDto
    {
        public IFormFile Imagefile { get; set; }
        public int? DisplayOrder { get; set; } 
    }
    public class CreateProductBaseVariantDto
    {
        public int? variantid {  get; set; }
        public string VariantName { get; set; }
        public decimal basicprice { get; set; }
        public string Description { get; set; }
        public string Mainimageurl { get; set; }
       // [JsonIgnore]
        public IFormFile Imagefile { get; set; }
        public int stockquantity { get; set; }
        public decimal discountpercentage { get; set; }
      //  public decimal FinalPrice { get; set; }
        //public bool? isaveliable { get; set; }
        public string sku {  get; set; }
      //  public int product_id { get; set; }
        public bool isdefalult {  get; set; }
        public List<ProductVariantAttributeDto> productVariantAttributeDtos { get; set; }
    }
    public class ProductVariantAttributeDto
    {
        public int VariantAttributeId { get; set; }
        public int VariantId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public string possiblevalue {  get; set; }
    }

    public class CreateProductAttributeValueDto
    {
        public int ProductId { get; set; }
        public int AttributeId { get; set; }
        public string Value { get; set; }
     }
     
}
