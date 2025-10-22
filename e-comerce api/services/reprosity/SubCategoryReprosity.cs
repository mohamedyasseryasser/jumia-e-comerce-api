using e_comerce_api.DTO;
using e_comerce_api.models;
using e_comerce_api.services.interfaces;
using Microsoft.EntityFrameworkCore;

namespace e_comerce_api.services.reprosity
{
    public class SubCategoryReprosity:IsubCategoryReprosity
    {
        private readonly Iimagesreprosity _imagesRepo;
        private readonly context _context;

        public SubCategoryReprosity(Iimagesreprosity imagesRepo, context context)
        {
            _imagesRepo = imagesRepo;
            _context = context;
        }
        public async Task<PagedResult<SubCategoryDto>> GetAllSubCategoryAsync(int pageNumber, int pageSize, bool isactive)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var totalitems = await _context.SubCategories.CountAsync();
                var totalpages = (int)Math.Ceiling(totalitems / (double)pageSize);

                IEnumerable<SubCategory> subCategories = await _context.SubCategories.ToListAsync();

                if (isactive)
                    subCategories = subCategories.Where(c => c.IsActive == true).ToList();
                else
                    subCategories = subCategories.Where(c => c.IsActive == false).ToList();

                List<SubCategoryDto> dto = subCategories.Select(c => new SubCategoryDto
                {
                    Name = c.Name,
                    description = c.description,
                    image = c.image,
                    IsActive = c.IsActive,
                    cat_id = c.cat_id
                }).ToList();

                await transaction.CommitAsync();

                return new PagedResult<SubCategoryDto>
                {
                    TotalItems = totalitems,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalpages,
                    Data = dto
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Error retrieving subcategories: {ex.Message}");
            }
        }
        // ✅ Get by Id
        public async Task<SubCategoryDto> GetByIdAsync(int id)
        {
            var result = await _context.SubCategories
                .Include(s => s.category)
                .FirstOrDefaultAsync(s => s.SbuCatId == id);

            if (result == null)
                throw new Exception("This subcategory is not found");

            return new SubCategoryDto
            {
                Name = result.Name,
                description = result.description,
                image = result.image,
                IsActive = result.IsActive,
                cat_id = result.cat_id
            };
        }
        // ✅ Search by Name
        public async Task<IEnumerable<SubCategoryDto>> SearchByNameAsync(string name, bool includeisinactive = false)
        {
            var result = await _context.SubCategories
                .Where(s =>
                    (s.Name.ToLower().Contains(name.ToLower()) || s.description.Contains(name)) &&
                    (s.IsActive == true || includeisinactive)
                )
                .ToListAsync();

            if (result == null || !result.Any())
                throw new Exception("No subcategories found");

            return result.Select(s => new SubCategoryDto
            {
                Name = s.Name,
                description = s.description,
                image = s.image,
                IsActive = s.IsActive,
                cat_id = s.cat_id
            }).ToList();
        }
        // ✅ Create
        public async Task<SubCategoryResponse> CreateAsync(SubCategoryDto dto)
        {
            try
            {
                var subCategory = new SubCategory
                {
                    Name = dto.Name,
                    description = dto.description,
                    image = dto.image,
                    IsActive = dto.IsActive,
                    cat_id = dto.cat_id
                };

                await _context.SubCategories.AddAsync(subCategory);
                await _context.SaveChangesAsync();

                return new SubCategoryResponse
                {
                    SubCatId = subCategory.SbuCatId,
                    Name = subCategory.Name,
                    description = subCategory.description,
                    image = subCategory.image,
                    IsActive = subCategory.IsActive,
                    cat_id = subCategory.cat_id
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        // ✅ Update
        public async Task<SubCategoryResponse> UpdateAsync(int id, SubCategoryDto dto)
        {
            var existing = await _context.SubCategories.FirstOrDefaultAsync(s => s.SbuCatId == id);
            if (existing == null)
                throw new Exception("This subcategory is not found");

            existing.Name = dto.Name;
            existing.description = dto.description;
            existing.IsActive = dto.IsActive;
            existing.cat_id = dto.cat_id;

            await _context.SaveChangesAsync();

            return new SubCategoryResponse
            {
                SubCatId = existing.SbuCatId,
                Name = existing.Name,
                description = existing.description,
                image = existing.image,
                IsActive = existing.IsActive,
                cat_id = existing.cat_id
            };
        }
        // ✅ Update Image URL
        public async Task<SubCategoryResponse> UpdateImageUrlAsync(int id, string imagepath)
        {
            var existing = await _context.SubCategories.FirstOrDefaultAsync(s => s.SbuCatId == id);
            if (existing == null)
                throw new Exception("This subcategory is not found");

            existing.image = imagepath;
            await _context.SaveChangesAsync();

            return new SubCategoryResponse
            {
                SubCatId = existing.SbuCatId,
                Name = existing.Name,
                description = existing.description,
                image = existing.image,
                IsActive = existing.IsActive,
                cat_id = existing.cat_id
            };
        }
        // ✅ Delete (make inactive)
        public async Task<bool> DeleteAsync(int id)
        {
            var subCategory = await _context.SubCategories.FirstOrDefaultAsync(s => s.SbuCatId == id);
            if (subCategory == null)
                throw new Exception("This subcategory is not found");

            subCategory.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }

}
