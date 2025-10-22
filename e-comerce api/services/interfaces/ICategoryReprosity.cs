using e_comerce_api.DTO;
using e_comerce_api.models;

namespace e_comerce_api.services.interfaces
{
    public interface ICategoryReprosity
    {
        Task<PagedResult<CategoryDto>> GetAllCategoryAsync(int pageNumber, int pageSize,bool isactive);
        Task<CategoryDto> GetByIdAsync(int id);
        Task<IEnumerable<CategoryDto>> SearchByNameAsync(string name, bool includeisinactive = false);
    Task<categoryresponse> CreateAsync(CategoryDto dto);
        Task<categoryresponse> UpdateAsync(int id, CategoryDto entity);
        Task<bool> DeleteAsync(int id);
        Task<categoryresponse> updateimageurl(int id, string imagepath);
    }
}
