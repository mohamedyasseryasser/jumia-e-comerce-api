
using e_comerce_api.DTO;
using e_comerce_api.Enum;
using e_comerce_api.models;
using e_comerce_api.services.interfaces;
using e_comerce_api.services.reprosity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace e_comerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        public SubCategoryController(Iimagesreprosity iimagesreprosity,IsubCategoryReprosity subCategoryReprosity)
        {
            Iimagesreprosity = iimagesreprosity;
            SubCategoryReprosity = subCategoryReprosity;
        }

        public Iimagesreprosity Iimagesreprosity { get; }
        public IsubCategoryReprosity SubCategoryReprosity { get; }

        private bool IsAdmin()
        {
            return User.IsInRole("admin");
        }
        private bool IsSeller()
        {
            return User.IsInRole("seller");
        }

        // ✅ Get all SubCategories
        [HttpGet("GetAllSubCategory")]
        public async Task<IActionResult> GetAllSubCategory(int pagenumber = 1, int pagesize = 5, bool isactive = true)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "Invalid data",
                        ErrorMessages = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToArray()
                    });
                }

                if (isactive != true && !IsAdmin())
                {
                    return BadRequest("You are not Admin");
                }

                if (pagenumber <= 0 || pagesize <= 0)
                    return BadRequest("Page number and page size must be greater than zero.");

                var subcategories = await SubCategoryReprosity.GetAllSubCategoryAsync(pagenumber, pagesize, isactive);

                foreach (var subcategory in subcategories.Data)
                {
                    subcategory.image = Iimagesreprosity.GetImageUrl(subcategory.image);
                }

                return Ok(subcategories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ✅ Get by ID
        [HttpGet("{id:int}/getsubcategorybyid")]
        public async Task<IActionResult> GetSubCategoryById(int id, bool includeinactive = false)
        {
            try
            {
                var result = await SubCategoryReprosity.GetByIdAsync(id);
                if (result == null)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "SubCategory not found"
                    });
                }

                if (!string.IsNullOrEmpty(result.image) && result.image.Length > 0)
                {
                    result.image = Iimagesreprosity.GetImageUrl(result.image);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // ✅ Search by term
        [HttpGet("{term:alpha}/searchbyterm")]
        public async Task<IActionResult> SearchByTerm(string term, bool includeinactive)
        {
            try
            {
                if (string.IsNullOrEmpty(term))
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "Term is not found"
                    });
                }

                var result = await SubCategoryReprosity.SearchByNameAsync(term, includeinactive);
                if (result == null)
                {
                    return NotFound("This term is not found");
                }

                foreach (var item in result)
                {
                    if (!string.IsNullOrEmpty(item.image) && item.image.Length > 0)
                    {
                        item.image = Iimagesreprosity.GetImageUrl(item.image);
                    }
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        // ✅ Create SubCategory
        [Authorize]
        [HttpPost("createnewsubcategory")]
        public async Task<IActionResult> CreateSubCategory([FromForm] SubCategoryDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "Invalid data",
                        ErrorMessages = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToArray()
                    });
                }

                if (!IsAdmin() && !IsSeller())
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "You are not Admin or Seller"
                    });
                }

                var result = await SubCategoryReprosity.CreateAsync(dto);
                if (result == null)
                    return NotFound("Creation failed");

                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    if (file != null && file.Length > 0)
                    {
                        var name = $"{result.Name}-{result.SubCatId}";
                        string imagepath = await Iimagesreprosity.saveimageurl(file, EntityTyp.subcategory, name);
                        result = await SubCategoryReprosity.UpdateImageUrlAsync(result.SubCatId, imagepath);
                        result.image = Iimagesreprosity.GetImageUrl(imagepath);
                    }
                }

                return CreatedAtAction(
                    nameof(GetSubCategoryById),
                    new { id = result.SubCatId },
                    new ApiResponse<SubCategoryResponse>
                    {
                        message = "Created successfully",
                        statue = true,
                        data = result
                    }
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // ✅ Update SubCategory
        [Authorize]
        [HttpPut("updatesubcategory")]
        public async Task<IActionResult> UpdateSubCategory(int subcatid, [FromForm] SubCategoryDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "Invalid data",
                        ErrorMessages = ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage)
                            .ToArray()
                    });
                }

                if (!IsAdmin() && !IsSeller())
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "You are not Admin or Seller"
                    });
                }

                var existSubCat = await SubCategoryReprosity.GetByIdAsync(subcatid);
                if (existSubCat == null)
                    return NotFound("This subcategory is not found");

                var result = await SubCategoryReprosity.UpdateAsync(subcatid, dto);
                if (result == null)
                {
                    return BadRequest(new ApiErrorResponse { Message = "No data to update" });
                }

                if (Request.Form.Files.Count > 0)
                {
                    var file = Request.Form.Files[0];
                    if (file != null && file.Length > 0)
                    {
                        var name = $"{result.Name}-{result.SubCatId}";
                        string imagepath = await Iimagesreprosity.saveimageurl(file, EntityTyp.subcategory, name);
                        result = await SubCategoryReprosity.UpdateImageUrlAsync(result.SubCatId, imagepath);
                        result.image = Iimagesreprosity.GetImageUrl(imagepath);
                    }
                }

                return Ok(new ApiResponse<SubCategoryResponse>
                {
                    message = "Updated successfully",
                    statue = true,
                    data = result
                });
            }
            catch
            {
                return NotFound(new ApiResponse<SubCategoryResponse>
                {
                    data = null,
                    message = "SubCategory not found",
                    statue = false
                });
            }
        }

        // ✅ Delete SubCategory
        [HttpDelete("deletesubcategory")]
        public async Task<IActionResult> DeleteSubCategory(int subcatid)
        {
            try
            {
                var existSubCat = await SubCategoryReprosity.GetByIdAsync(subcatid);
                if (existSubCat == null)
                {
                    return NotFound("This subcategory is not found");
                }

                var result = await SubCategoryReprosity.DeleteAsync(subcatid);
                if (result == true)
                {
                    if (!string.IsNullOrEmpty(existSubCat.image) && existSubCat.image.Length > 0)
                    {
                        await Iimagesreprosity.deleteimage(existSubCat.image);
                    }
                }

                return Ok($"{existSubCat.Name} deleted successfully");
            }
            catch (Exception ex)
            {
                return NotFound(new ApiResponse<SubCategoryResponse>
                {
                    data = null,
                    message = "SubCategory not found",
                    statue = false
                });
            }
        }
    }
}
