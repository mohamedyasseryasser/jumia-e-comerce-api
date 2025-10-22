using e_comerce_api.Enum;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace e_comerce_api.services.interfaces
{
    public interface Iimagesreprosity
    {
        Task<string> saveimageurl(IFormFile formFile,EntityTyp entityType, string name);
        Task<string> savestreamimage(Stream imageStream, EntityTyp entityType, string entityName);
        Task<bool> deleteimage(string urlimage);
        Task<string> UpdateImageAsync(IFormFile newImageFile, string oldImagePath, EntityTyp entityType, string name);
        string GetImageUrl(string relativePath);
        string GenerateUniqueFileName(string originalFileName, string extension);
        string SanitizeFileName(string filename);
    }
}
