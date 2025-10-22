using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.DTO
{
    //using System.ComponentModel.DataAnnotations;

    //  namespace Jumia_Clone.Models.DTOs.ProductVariantDTOs

    public class CreateProductVariantDto
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string VariantName { get; set; }
        [Required]
        public string description {  get; set; }
        [Required]
        [Range(0.01, 99999999.99)]
        public decimal Price { get; set; }
        public decimal finalprice {  get; set; }
        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        [StringLength(50)]
        public string Sku { get; set; }

        public IFormFile VariantImageFile { get; set; }

        public bool IsDefault { get; set; }

        public bool IsAvailable { get; set; }
        // Add this field to hold the JSON string
        public string VariantAttributesJson { get; set; }

        // Keep the original property but don't expect it to be bound from the form
        [System.Text.Json.Serialization.JsonIgnore]
        public List<ProductVariantAttributeDto> VariantAttributes { get; set; } = new List<ProductVariantAttributeDto>();
    }
    public class productvariantresponsedto
    {
        public int VariantId { get; set; }
        public int ProductId { get; set; }
        public string VariantName { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal finalprice { get; set; }
        public int StockQuantity { get; set; }
        public string Sku { get; set; }
        public string VariantImageUrl { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsAvailable { get; set; }
        public List<ProductVariantAttributeDto> VariantAttributes { get; set; } = new List<ProductVariantAttributeDto>();

    }
}
    
