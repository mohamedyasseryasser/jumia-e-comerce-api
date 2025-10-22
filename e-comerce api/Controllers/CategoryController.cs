using e_comerce_api.DTO;
using e_comerce_api.Enum;
using e_comerce_api.services.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Immutable;
using System.Threading.Tasks;
using e_comerce_api.models;

namespace e_comerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        public CategoryController(Iimagesreprosity iimagesreprosity, ICategoryReprosity categoryReprosity)
        {
            Iimagesreprosity = iimagesreprosity;
            CategoryReprosity = categoryReprosity;
        }

        public Iimagesreprosity Iimagesreprosity { get; }
        public ICategoryReprosity CategoryReprosity { get; }
        private bool IsAdmin()
        {
            return User.IsInRole("admin");
        }
        private bool IsSeller()
        {
            return User.IsInRole("seller");
        }
        [HttpGet("GetAllCategory")]
        public async Task<IActionResult> GetAllCategory(int pagenumber = 1, int pagesize = 5, bool isactive = true)
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
                    return BadRequest("your are not Admin");
                }
                if (pagenumber <= 0 || pagesize <= 0)
                    return BadRequest("Page number and page size must be greater than zero.");
                var categories = await CategoryReprosity.GetAllCategoryAsync(pagenumber, pagesize, isactive);
                foreach (var category in categories.Data)
                {
                    category.images = Iimagesreprosity.GetImageUrl(category.images);
                }
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("{id:int}/getcategorybyid")]
        public async Task<IActionResult> getcategorybyid(int id, bool includeinactive = false)
        {
            try
            {
                var result = await CategoryReprosity.GetByIdAsync(id);
                if (result == null)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "Category not found"
                    });
                }
                if (!string.IsNullOrEmpty(result.images) && result.images.Length > 0)
                {
                    result.images = Iimagesreprosity.GetImageUrl(result.images);
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [HttpGet("{term:alpha}/searchbyterm")]
        public async Task<IActionResult> searchbyterm(string term, bool includeinactive)
        {
            try
            {
                if (string.IsNullOrEmpty(term))
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "is not fount"
                    });
                }
                var result = await CategoryReprosity.SearchByNameAsync(term, includeinactive);
                if (result == null)
                {
                    return NotFound("this term is not found");
                }
                foreach (var item in result)
                {
                    if (!string.IsNullOrEmpty(item.images) && item.images.Length > 0)
                    {
                        item.images = Iimagesreprosity.GetImageUrl(item.images);
                    }
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        [Authorize]
        [HttpPost("createnewcategory")]
        public async Task<IActionResult> createcategory([FromForm] CategoryDto dto)
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
                if (!IsAdmin() || !IsSeller())
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "your are not Admin or seller"
                    });
                }
                var result = await CategoryReprosity.CreateAsync(dto);
                if (result == null)
                    return NotFound("created cat is falid");
                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    var name = $"{result.Name}-{result.categoryid}";
                    string imagepath = await Iimagesreprosity.saveimageurl(dto.ImageFile, EntityTyp.category, name);
                    result = await CategoryReprosity.updateimageurl(result.categoryid, imagepath);
                    result.images = Iimagesreprosity.GetImageUrl(imagepath);
                }
                return CreatedAtAction(
                    nameof(getcategorybyid),
                    new { id = result.categoryid }
                   , new ApiResponse<categoryresponse>
                   {
                       message = "created is successfully",
                       statue = true,
                       data = result,
                   }
                    );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpPut("updatecategory")]
        public async Task<IActionResult> updatecategory(int catid, CategoryDto dto)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "invalid data",
                        ErrorMessages = ModelState.Values.
                        SelectMany(v => v.Errors).
                        Select(e => e.ErrorMessage).ToArray()
                    });
                }
                if (!IsAdmin() || !IsSeller())
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Message = "your are not Admin or seller"
                    });
                }
                var exitcat = await CategoryReprosity.GetByIdAsync(catid);
                if (exitcat == null)
                    return NotFound("this cat is not found");
                var result = await CategoryReprosity.UpdateAsync(catid, dto);
                if (result == null)
                {
                    return BadRequest(new ApiErrorResponse { Message = "there are not data" });
                }
                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    var name = $"{result.Name}-{result.categoryid}";
                    string imagepath = await Iimagesreprosity.saveimageurl(dto.ImageFile, EntityTyp.category, name);
                    result = await CategoryReprosity.updateimageurl(result.categoryid, imagepath);
                    result.images = Iimagesreprosity.GetImageUrl(imagepath);
                }
                var updateresponsedto = new updatecatresponsedto
                {
                    categoryid = result.categoryid,
                    Description = result.Description,
                    ImageFile = result.ImageFile,
                    images = result.images,
                    IsActive = result.IsActive,
                    Name = result.Name,
                };
                return Ok(updateresponsedto);
            }
            catch
            {
                return NotFound(new ApiResponse<categoryresponse>
                {
                    data = null,
                    message = "cat is not found",
                    statue = false
                });
            }
        }
        [HttpDelete("deletcategory")]
        public async Task<IActionResult> deletcategory(int catid)
        {
            try
            {
                var exitcat = await CategoryReprosity.GetByIdAsync(catid);
                if (exitcat == null)
                {
                    return NotFound("this category is not found");
                }
                var result = await CategoryReprosity.DeleteAsync(catid);
                if (result == true)
                {
                    if (!string.IsNullOrEmpty(exitcat.images) && exitcat.images.Length > 0)
                    {
                        var deletedimage =await Iimagesreprosity.deleteimage(exitcat.images);

                    }
                }
                return Ok($"{exitcat.Name} is deleted with successfully");
            }
            catch (Exception ex)
            {
                {
                    return BadRequest(ex.Message);
                }
            }
        }
    }
}