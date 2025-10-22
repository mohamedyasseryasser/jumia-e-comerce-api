using e_comerce_api.DTO;
using e_comerce_api.models;
using e_comerce_api.services.interfaces;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Threading.Tasks;

namespace e_comerce_api.services.reprosity
{
    public class Categoryreprosity: ICategoryReprosity
    {
        public Categoryreprosity(Iimagesreprosity iimagesreprosity,context context) 
        {
            Iimagesreprosity = iimagesreprosity;
            Context = context;
        }

        public Iimagesreprosity Iimagesreprosity { get; }
        public context Context { get; }

        public async Task<PagedResult<CategoryDto>> GetAllCategoryAsync(int pageNumber, int pageSize,bool isactive)
        {
            using var transaction = await Context.Database.BeginTransactionAsync();
            try
            {
                
                //pagination
                var totalitems = await Context.Categories.CountAsync();
                var totalpages = (int)Math.Ceiling(totalitems / (double)pageSize);
                //represent data
                IEnumerable<category> categories = await Context.Categories.ToListAsync();
                if (isactive==true) { 
                   categories=categories.Where(c=>c.IsActive==true).ToList();
                }
                else
                {
                    categories=categories.Where(c=>c.IsActive==false).ToList();
                }
                    List<CategoryDto> dto = categories.Select(c => new CategoryDto
                    {
                        Name = c.Name,
                        Description = c.Description,
                        images = c.images,
                        IsActive = c.IsActive,
                    }).ToList();
                await transaction.CommitAsync();
                return new PagedResult<CategoryDto>
                {
                    TotalItems = totalitems,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalpages,
                    Data = dto
                };
            }
            catch (Exception ex) { 
              await transaction.RollbackAsync();
                throw new Exception($"Error retrieving customers: {ex.Message}");
            }
        }
        //get category  by id
        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            try
            {
                var result = await Context.Categories
                    .Include(c => c.subCategories)
                    .FirstOrDefaultAsync(c => c.CategoryId == id);
                if (result == null)
                {
                    throw new Exception("this cat is not found");
                }
                var dto = new CategoryDto { 
                 Description = result.Description,
                 images= result.images,
                 IsActive= result.IsActive,
                 Name = result.Name,
                };
                return dto;
            }
            catch (Exception ex) {
                throw new Exception(ex.Message);   
            }
            }
        // search by cat name
        public async Task<IEnumerable<CategoryDto>> SearchByNameAsync(string name,bool includeisinactive=false)
        {
            try { 
            var result = await Context.Categories
                .Where(c => 
                (c.Name.ToLower().Contains(name.ToLower())||c.Description.Contains(name))&&
                 (c.IsActive==true ||includeisinactive)
                )
                .ToListAsync();
            if (result == null)
            {
                throw new Exception("this cat is not found");
            }
                var dto = result.Select(c=>new CategoryDto { 
                Description=c.Description,
                images=c.images,
                IsActive = c.IsActive,
                Name = c.Name,
                }).ToList();
            return dto;
        }
            catch (Exception ex) {
                throw new Exception(ex.Message);
    }
}
        //create new category
        public async Task<categoryresponse> CreateAsync(CategoryDto dto)
        {
             try
            {
                category category = new category { 
                 IsActive = dto.IsActive,
                 Name = dto.Name,
                 Description=dto.Description,
                 images = dto.images,
                 subCategories=new List<SubCategory>()
                };
                await Context.Categories.AddAsync(category);
                await Context.SaveChangesAsync();

                return new  categoryresponse{
                 Description=category.Description,
                 categoryid=category.CategoryId,
                 images=category.images,
                 IsActive=category.IsActive,
                 Name=category.Name,
                }
                ;
            }
            catch (Exception ex) { 
                 throw new Exception(ex.Message);
            }
        }
        // update category
        public async Task<categoryresponse> UpdateAsync(int id, CategoryDto entity)
        {
            var existing = await Context.Categories.FirstOrDefaultAsync(c=>c.CategoryId==id);
            if (existing == null)
                throw new Exception("this category is not found");

            existing.Name = entity.Name;
            existing.Description = entity.Description;
            existing.IsActive = entity.IsActive;
            //existing.images = entity.images;

            await Context.SaveChangesAsync();

            return new categoryresponse
            {
                Description = existing.Description,
                categoryid = existing.CategoryId,
                images = existing.images,
                IsActive = existing.IsActive,
                Name = existing.Name,
            };
        }
        //update category imageurl
        public async Task<categoryresponse> updateimageurl(int id,string imagepath)
        {
            var existing = await Context.Categories.FirstOrDefaultAsync(c => c.CategoryId == id);
            if (existing == null)
                throw new Exception("this category is not found");
            existing.images= imagepath;
            await Context.SaveChangesAsync();
            return new categoryresponse
            {
                Description = existing.Description,
                categoryid = existing.CategoryId,
                images = existing.images,
                IsActive = existing.IsActive,
                Name = existing.Name,
            };
        }
        //delete category
        public async Task<bool> DeleteAsync(int id)
        {
            var category = await Context.Categories.FirstOrDefaultAsync(c=>c.CategoryId==id);
            if (category == null)
                throw new Exception("this category is not found");

            category.IsActive = false;
            await Context.SaveChangesAsync();
            return true;
        }
    }
}
