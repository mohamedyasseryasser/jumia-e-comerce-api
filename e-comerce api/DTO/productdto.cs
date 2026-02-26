using e_comerce_api.models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace e_comerce_api.DTO
{
    public class productsdto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public double AverageRate { get; set; }
        public decimal FinalPrice { get; set; }
        public int StockQuantity { get; set; }
        public string mainimageurl {  get; set; }
    }
    
       
    public class productresponsedto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal FinalPrice { get; set; }
        public int StockQuantity { get; set; }
        public bool IsAvailable { get; set; }
       // public string ApprovalStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string MainImageUrl { get; set; }
        public double AverageRating { get; set; }
        public int SellerId { get; set; }
        public string SellerName { get; set; }
        public int SubcategoryId { get; set; }
        public string SubcategoryName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int RatingCount { get; set; }
        public int ReviewCount { get; set; }
        public List<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
        public List<ProductVariantDto> Variants { get; set; }
        public List<ProductAttributeValueDto> AttributeValues { get; set; }
    }
     
        public class ProductImageDto
        {
            public int ImageId { get; set; }
            public int ProductId { get; set; }
            public string ImageUrl { get; set; }
            public int? DisplayOrder { get; set; }
        }
   public class ProductVariantDto
    {
        public int VariantId { get; set; }
        public int ProductId { get; set; }
        public string VariantName { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public decimal DiscountedPrice => Price - (Price * (DiscountPercentage ?? 0) / 100);
        public int StockQuantity { get; set; }
        public decimal finalprice{ get; set; }
        public string VariantImageUrl { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsAvailable { get; set; }
        public List<UpdateVariantAttributeDto> VariantAttributes { get; set; } = new List<UpdateVariantAttributeDto>();
    }

    public class UpdateVariantAttributeDto
    {
        public int? VariantId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Value { get; set; }
    }
    public class ProductAttributeValueDto
    {
        public int ValueId { get; set; }
        public int ProductId { get; set; }
        public int AttributeId { get; set; }
        public string AttributeName { get; set; }
        public string AttributeType { get; set; }
        public string Value { get; set; }
    }
}

