using e_comerce_api.DTO;
using e_comerce_api.Enum;
using e_comerce_api.models;
using e_comerce_api.services.interfaces;
using e_comerce_api.services.reprosity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Threading.Tasks;

namespace e_comerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public ProductController(
            IUser Iuser
            ,Iimagesreprosity iimagesreprosity,
            Iproductreprosity iproductreprosity)
        {
            this.Iuser = Iuser;
            Iimagesreprosity = iimagesreprosity;
            Iproductreprosity = iproductreprosity;
        }

        public IUser Iuser { get; }
        public Iimagesreprosity Iimagesreprosity { get; }
        public Iproductreprosity Iproductreprosity { get; }
        //       public object Newtonsoft { get; private set; }

        private bool IsAdmin()
        {
            return User.IsInRole("admin");
        }
        [HttpGet("getproductbyid")]
        public async Task<IActionResult> Getproductbyid(int productid, [FromQuery] bool isincludeddetails = true)
        {
            try
            {
                var product = await Iproductreprosity.getproductbyid(productid, isincludeddetails);
                if (product == null)
                    return NotFound(new ApiErrorResponse
                    {
                        Message = "this product is not found",
                    });
                if (string.IsNullOrEmpty(product.MainImageUrl))
                {
                    product.MainImageUrl = Iimagesreprosity.GetImageUrl(product.MainImageUrl);
                }
                if (isincludeddetails == true && product.Images != null)
                {
                    foreach (var image in product.Images)
                    {
                        image.ImageUrl = Iimagesreprosity.GetImageUrl(image.ImageUrl);
                    }
                }
                if (isincludeddetails == true && product.Variants != null)
                {
                    foreach (var variant in product.Variants)
                    {
                        variant.VariantImageUrl = Iimagesreprosity.GetImageUrl(variant.VariantImageUrl);
                    }
                }
                return Ok(new ApiResponse<productresponsedto>
                {
                    data = product,
                    message = "successfully",
                    statue = true,
                });
            }
            catch (Exception ex)
            {
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "An error occurred while retrieving the product",
                        ErrorMessages = new string[] { ex.Message }
                    });
                }
            }
        }
        //create new product 
        [HttpPost("addnewproduct")]
        //        [Consumes("")]
        [Consumes("multipart/form-data")]

        public async Task<IActionResult> addnewproduct([FromForm] CreateProductInputDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Invalid product data",
                    ErrorMessages = ModelState.Values
                       .SelectMany(v => v.Errors)
                       .Select(e => e.ErrorMessage)
                       .ToArray()
                });
            }
            try
            {


                // Process attribute values JSON
                if (!string.IsNullOrEmpty(dto.attributesvaluesjson))
                {
                    try
                    {
                        //  if (dto.attributesvaluesjson.StartsWith("'"))
                        //{
                        //  dto.attributesvaluesjson = dto.attributesvaluesjson.Substring(1);
                        //}
                        //if (dto.attributesvaluesjson.EndsWith("'"))
                        //{
                        //  dto.attributesvaluesjson = dto.attributesvaluesjson.Substring(0, dto.attributesvaluesjson.Length - 1);
                        //}
                        var json = dto.attributesvaluesjson.Trim();

                        if (json.StartsWith("\"") && json.EndsWith("\""))
                        {
                            json = json.Substring(1, json.Length - 2);
                            json = json.Replace("\\\"", "\""); // يشيل الـ escape characters
                        }

                        var data = dto.attributesvaluesjson;
                        var singleAttribute = System.Text.Json.JsonSerializer.Deserialize<CreateProductAttributeValueDto>(dto.attributesvaluesjson);
                        dto.AttributeValues = new List<CreateProductAttributeValueDto> { singleAttribute };

                        // dto.AttributeValues = System.Text.Json.JsonSerializer.Deserialize<List<CreateProductAttributeValueDto>>(dto.attributesvaluesjson);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                // Process product variants JSON
                if (dto.HasVariants && !string.IsNullOrEmpty(dto.ProductVariantsJson))
                {
                    try
                    {
                        //if (dto.ProductVariantsJson.StartsWith("'"))
                        // {
                        //   dto.ProductVariantsJson = dto.ProductVariantsJson.Substring(1);
                        //}
                        //if (dto.ProductVariantsJson.EndsWith("'"))
                        //{
                        //  dto.ProductVariantsJson = dto.ProductVariantsJson.Substring(0, dto.ProductVariantsJson.Length - 1);
                        // }
                        var json = dto.ProductVariantsJson.Trim();

                        if (json.StartsWith("\"") && json.EndsWith("\""))
                        {
                            json = json.Substring(1, json.Length - 2);
                            json = json.Replace("\\\"", "\""); // يشيل الـ escape characters
                        }

                        //   var data = dto.ProductVariantsJson;
                        // dto.Variants = System.Text.Json.JsonSerializer.Deserialize<List<CreateProductBaseVariantDto>>(dto.ProductVariantsJson);
                        var data = dto.attributesvaluesjson;
                        var singlevarient = System.Text.Json.JsonSerializer.Deserialize<CreateProductBaseVariantDto>(dto.ProductVariantsJson);
                        dto.Variants = new List<CreateProductBaseVariantDto> { singlevarient };
                        // Validate at least one variant is marked as default
                        if (!dto.Variants.Any(v => v.isdefalult))
                        {
                            return BadRequest(new ApiErrorResponse
                            {
                                Message = "At least one variant must be marked as default",
                                ErrorMessages = new string[] { "No default variant specified" }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new ApiErrorResponse
                        {
                            Message = "Invalid product variants data",
                            ErrorMessages = new string[] { ex.Message }
                        });
                    }
                }


                //create new product
                var product = await Iproductreprosity.createproduct(dto, IsAdmin());

                //handle mainimage for product
                if (dto.MainImageFile != null && dto.MainImageFile.Length > 0)
                {
                    var mainimagepath = await Iimagesreprosity.
                          saveimageurl(dto.MainImageFile
                          , EntityTyp.product,
                          product.Name);
                    product = await Iproductreprosity.UpdateProductMainImageAsync(product.ProductId, mainimagepath);

                    product.MainImageUrl = Iimagesreprosity.GetImageUrl(mainimagepath);
                }
                //handle addtional image for product
                if (dto.AdditionalImageFiles != null && dto.AdditionalImageFiles.Any())
                {

                    try
                    {
                        foreach (var image in dto.AdditionalImageFiles)
                        {


                            var imagedetails = await Iproductreprosity.addimagetoproductimages(image, product.ProductId);
                            product.Images.Add(imagedetails);
                        }
                        product = await Iproductreprosity.getproductbyid(product.ProductId, IsAdmin());
                        if (product.Images != null)
                        {
                            foreach (var image in product.Images)
                            {
                                image.ImageUrl = Iimagesreprosity.GetImageUrl(image.ImageUrl);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error while adding additional images: {ex.Message}", ex);

                    }
                }
                return CreatedAtAction(
                    nameof(Getproductbyid),
                    new { id = product.ProductId },
                    new ApiResponse<productresponsedto>
                    {
                        data = product,
                        message = IsAdmin() ? "Product created and approved successfully" : "Product created successfully (pending approval)",
                        statue = true
                    }
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while creating the product",
                    ErrorMessages = new string[]
                    {
            ex.Message,
            ex.InnerException?.Message ?? "No inner exception",
            ex.StackTrace
                    }
                });
            }
        }
        //update product
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> updateproduct(int productid, [FromForm] updateproductinputdto dto)
        {
            if (dto.productid != productid)
            {
                return BadRequest("product is not equel dto.productid");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Invalid product data",
                    ErrorMessages = ModelState.Values
                       .SelectMany(v => v.Errors)
                       .Select(e => e.ErrorMessage)
                       .ToArray()
                });
            }
            try
            {


                // Process attribute values JSON
                if (!string.IsNullOrEmpty(dto.attributesvaluesjson))
                {
                    try
                    {
                        if (dto.attributesvaluesjson.StartsWith("'"))
                        {
                            dto.attributesvaluesjson = dto.attributesvaluesjson.Substring(1);
                        }
                        if (dto.attributesvaluesjson.EndsWith("'"))
                        {
                            dto.attributesvaluesjson = dto.attributesvaluesjson.Substring(0, dto.attributesvaluesjson.Length - 1);
                        }
                        dto.AttributeValues = System.Text.Json.JsonSerializer.Deserialize<List<CreateProductAttributeValueDto>>(dto.attributesvaluesjson);

                    }

                    catch (Exception)
                    {
                        throw;
                    }
                }

                // Process product variants JSON
                if (dto.HasVariants && !string.IsNullOrEmpty(dto.ProductVariantsJson))
                {
                    try
                    {
                        if (dto.ProductVariantsJson.StartsWith("'"))
                        {
                            dto.ProductVariantsJson = dto.ProductVariantsJson.Substring(1);
                        }
                        if (dto.ProductVariantsJson.EndsWith("'"))
                        {
                            dto.ProductVariantsJson = dto.ProductVariantsJson.Substring(0, dto.ProductVariantsJson.Length - 1);
                        }
                        dto.Variants = System.Text.Json.JsonSerializer.Deserialize<List<UpdateProductBaseVariantDto>>(dto.ProductVariantsJson);

                        // Validate at least one variant is marked as default
                        if (!dto.Variants.Any(v => v.IsDefault))
                        {
                            return BadRequest(new ApiErrorResponse
                            {
                                Message = "At least one variant must be marked as default",
                                ErrorMessages = new string[] { "No default variant specified" }
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        return BadRequest(new ApiErrorResponse
                        {
                            Message = "Invalid product variants data",
                            ErrorMessages = new string[] { ex.Message }
                        });
                    }
                }


                var updatedproducte = await Iproductreprosity.updateproduct(dto);
                if (updatedproducte == false)
                {
                    return BadRequest("updated product is failed");
                }
                //check for product
                var exitproduct = await Iproductreprosity.getproductbyid(dto.productid, false);
                if (exitproduct == null)
                    return BadRequest("this product is not found");



                if (exitproduct.Images != null && exitproduct.Images.Any())
                {
                    exitproduct.MainImageUrl = Iimagesreprosity.GetImageUrl(exitproduct.MainImageUrl);
                }
                if (exitproduct.Images != null)
                {
                    foreach (var image in exitproduct.Images)
                    {
                        image.ImageUrl = Iimagesreprosity.GetImageUrl(image.ImageUrl);
                    }
                }

                if (exitproduct.Variants != null)
                {
                    foreach (var variant in exitproduct.Variants)
                    {
                        if (!string.IsNullOrEmpty(variant.VariantImageUrl))
                        {
                            variant.VariantImageUrl = Iimagesreprosity.GetImageUrl(variant.VariantImageUrl);
                        }
                    }
                }
                return Ok(new ApiResponse<productresponsedto>
                {
                    message = "Product updated successfully",
                    data = exitproduct,
                    statue = true
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while updating the product",
                    ErrorMessages = new[] { ex.Message }
                });
            }
        }
        //update product avaliablty
        [HttpPut("{id}/avaliablty")]
        public async Task<IActionResult> updateproductavaliablty(int productid,[FromBody] bool isavaliblty)
        {
            try
            {
                
                    var product = await Iproductreprosity.getproducttoupdate(productid);
                if (product == null) {
                    return NotFound(new ApiResponse<object>
                    {
                        message = "Product not found",
                        statue = false,
                        data = null
                    });
                }
                
               if (IsAdmin())
                {
                    var updated = await Iproductreprosity.updateproductavaliablity(productid, isavaliblty);
                    if (updated == false)
                    {
                        return BadRequest("updated is failed");
                    }
                }
                else
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "you are not admin",
                    });
                }
                return Ok(new ApiResponse<object>
                {
                    statue = true,
                    message = "update is successfully",
                    data = $"product is updated to {isavaliblty}"
                });
            }
            catch (Exception ex)
            {
                return NotFound(new ApiErrorResponse { Message=ex.Message});
            }
        }
        [HttpDelete("{id}/deleteproduct")]
        public async Task<IActionResult> deleteproduct(int id)
        {
            try
            {
               // if (!IsAdmin())
                //{
                  //  return BadRequest(new ApiErrorResponse
                    //{
                      //  Message="you are not admin"
                    //});
                //}
                var product = await Iproductreprosity.getproductbyid(id,false);
                if (product == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        message=$"product: {id} is not found",
                        statue=false,
                    }); 
                };
                var deleted = await Iproductreprosity.deletproduct(id);
                if (!deleted)
                {
                    return BadRequest("deleted is failed");
                }
                return Ok(new ApiResponse<object>
                {
                    statue = true,
                    message = $"{id}: is deleted",
                    data = $"product avalibilty become {product.IsAvailable}"
                });
            }
            catch (Exception ex)
            {
                return NotFound(new ApiErrorResponse
                {

                });
            }
        }
        [HttpGet("{sellerid}/getsellerproducts")]
        public async Task<IActionResult> getsellerproduct(int sellerid,[FromQuery] PaginationDto paginationdto,[FromQuery] Productfilterdto filter)

        {
            try
            {
                var exitseller =await Iuser.GetSellerByIdAsync(sellerid);
                if (exitseller == null)
                {
                    return NotFound(new ApiErrorResponse
                    {
                        Message=$"this seller :{sellerid} is not found",
                    });
                }
               var response= await Iproductreprosity.getsellerproduct(sellerid, paginationdto, filter);
                // Convert image paths to URLs
                foreach (var product in response)
                {
                    if (!string.IsNullOrEmpty(product.MainImageUrl))
                    {
                        product.MainImageUrl = Iimagesreprosity.GetImageUrl(product.MainImageUrl);
                    }
                }
                return Ok(new ApiResponse<IEnumerable<productresponsedto>> {
                    message=$"this is product for seller :{exitseller.name}",
                    statue=true,
                    data=response,
                });
            }
            catch (Exception ex)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = "An error occurred while retrieving seller products",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
        [HttpPost("addproductvariant")]
        public async Task<IActionResult> addproductvariant(CreateProductVariantDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Invalid product data",
                    ErrorMessages = ModelState.Values
                       .SelectMany(v => v.Errors)
                       .Select(e => e.ErrorMessage)
                       .ToArray()
                });
            }
            try
            {
                var exitproduct = await Iproductreprosity.getproductbyid(dto.ProductId,false);
                if (exitproduct == null)
                {
                    return NotFound("this product is not found");
                }
                var response = await Iproductreprosity.addproductvariant(dto);
                response.VariantImageUrl = Iimagesreprosity.GetImageUrl(response.VariantImageUrl);
                return Ok(new ApiResponse<productvariantresponsedto>
                {
                    statue = true,
                    message = "add variant is successfully",
                    data = response,
                });

            }
            catch (Exception ex)
            {
                return NotFound(new ApiErrorResponse
                {
                    ErrorMessages = new string[] { ex.Message },
                    Message = "add variant is failed"
                });
            }
        }
        [HttpPut("variants/{variantId}")]
        public async Task<IActionResult> updatevariant(int variantid,[FromForm]UpdateProductBaseVariantDto dto)
        {
            if (variantid!=dto.VariantId)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Invalid variant ID",
                    ErrorMessages = new string[] { "ID in URL does not match ID in request body" }
                });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiErrorResponse
                {
                    Message = "Invalid variant data",
                    ErrorMessages = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToArray()
                });
            }
            try
            {
                var updatevariant=await Iproductreprosity.updateproductvariant(dto);
                return Ok(new ApiResponse<productvariantresponsedto>
                {
                    data = updatevariant,
                    message = "Variant updated successfully",
                    statue = true
                });
            
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while updating the variant",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
        [HttpDelete("deletevariant")]
        public async Task<IActionResult> deletevariant(int variantid)
        {
            try
            {
               await Iproductreprosity.deleteproductvariant(variantid);
                return Ok(new ApiResponse<object>
                {
                    message = "Variant deleted successfully",
                    data = null,
                    statue = true
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    message = ex.Message,
                    statue = false,
                    data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while deleting the variant",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
        [HttpPost("{id}/stock")]
        public async Task<IActionResult> UpdateStock(int id, [FromBody]int stockquantity)
        {
            try
            {
                var updatedProduct = await Iproductreprosity.updateproductstock(id, stockquantity);

                return Ok(new ApiResponse<productresponsedto>
                {
                    data = updatedProduct,
                    message = "Stock updated successfully",
                    statue = true
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    message = ex.Message,
                    statue = false,
                    data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while updating the stock",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }

        // POST: api/products/variants/{variantId}/stock
        [HttpPost("variants/{variantId}/stock")]
        public async Task<IActionResult> UpdateVariantStock(int variantId, [FromBody]int stock)
        {
            try
            {
                var updatedVariant = await Iproductreprosity.UpdateVariantStockAsync(variantId, stock);

                return Ok(new ApiResponse<productvariantresponsedto>
                {
                    data = updatedVariant,
                    message = "Variant stock updated successfully",
                    statue = true
                });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>
                {
                    message = ex.Message,
                    statue = false,
                    data = null
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while updating the variant stock",
                    ErrorMessages = new string[] { ex.Message }
                });
            }
        }
    }
}