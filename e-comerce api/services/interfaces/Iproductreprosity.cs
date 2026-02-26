using e_comerce_api.DTO;

namespace e_comerce_api.services.interfaces
{    
         public interface Iproductreprosity
        {
          Task<productresponsedto> getproductbyid(int productid, bool isincludedetails);
          Task<IEnumerable<productresponsedto>> getallproduct(PaginationDto paginationdto, Productfilterdto filter = null);
         Task<productresponsedto> createproduct(CreateProductInputDto dto, bool IsAdmin = false);
        Task<bool> updateproduct(updateproductinputdto productdto);
        Task<bool> updateproductavaliablity(int productid, bool isavaliable);
        Task<productresponsedto> UpdateProductMainImageAsync(int id, string imagePath);
        Task<bool> deletproduct(int productid);
        Task<IEnumerable<productresponsedto>> getsellerproduct(int sellerid, PaginationDto paginationdto, Productfilterdto filter = null);
        Task<ProductImageDto> addimagetoproductimages(IFormFile dto, int productid);
        Task<productvariantresponsedto> addproductvariant(CreateProductVariantDto variant);
        Task<productvariantresponsedto> updateproductvariant(UpdateProductBaseVariantDto variant);
        Task<bool> deleteproductvariant(int variantid);
        Task<productresponsedto> getproducttoupdate(int productid);
        Task<productattributevalueresponse> addproductattributevalue(addproductattrvaluedto dto);
        Task UpdateProductAttributeValueAsync(int valueId, updateproductattributevaluedto attributeValueDto);
        Task<productresponsedto> updateproductstock(int productid, int stockquantity);
        Task<productvariantresponsedto> UpdateVariantStockAsync(int variantId, int newStock);
    }
}
