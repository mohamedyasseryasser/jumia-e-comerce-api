using e_comerce_api.DTO;

namespace e_comerce_api.services.interfaces
{
    public interface IsubCategoryReprosity
    {
        Task<PagedResult<SubCategoryDto>> GetAllSubCategoryAsync(int pageNumber, int pageSize, bool isactive);
        Task<SubCategoryDto> GetByIdAsync(int id);
        Task<IEnumerable<SubCategoryDto>> SearchByNameAsync(string name, bool includeisinactive = false);
        Task<SubCategoryResponse> CreateAsync(SubCategoryDto dto);
        Task<SubCategoryResponse> UpdateAsync(int id, SubCategoryDto dto);
        Task<SubCategoryResponse> UpdateImageUrlAsync(int id, string imagepath);
        Task<bool> DeleteAsync(int id);
    }
}
