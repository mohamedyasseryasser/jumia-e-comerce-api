using e_comerce_api.Enum;
using e_comerce_api.services.interfaces;
using e_comerce_api.services.reprosity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace e_comerce_api.services.reprosity
{
    public class Imagesreprosity:Iimagesreprosity
    {
        public Imagesreprosity(IWebHostEnvironment en) {
            En = en;
        }
        public IWebHostEnvironment En { get; }
        private readonly string _imagesFolder = "Images";
        public async Task<string> saveimageurl(IFormFile formFile,EntityTyp entityType,string name)
        {
            if (formFile==null||formFile.Length==0)
            {
                throw new Exception("please writem url");
            }
            string entityfolder = entityType.ToString()+"s";
            string santizefilename = SanitizeFileName(entityfolder);
            string uniqueimagename = GenerateUniqueFileName(formFile.FileName,Path.GetExtension(formFile.FileName));
            var directpath = Path.Combine(En.WebRootPath,_imagesFolder,entityfolder,santizefilename);
            if (!Directory.Exists(directpath))
            {
                Directory.CreateDirectory(directpath);
            }
            var filepath= Path.Combine(directpath,uniqueimagename);
            using (var stream = new FileStream(filepath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }
            // Return relative path from wwwroot
            return Path.Combine(_imagesFolder, entityfolder, santizefilename, uniqueimagename);
        }
        public string SanitizeFileName(string filename)
        {
            // Remove invalid file name characters
            string invalidChars = new string(Path.GetInvalidFileNameChars());
            string sanitized = filename;

            foreach (char c in invalidChars)
            {
                sanitized = sanitized.Replace(c.ToString(), "");
            }

            // Replace spaces with hyphens
            sanitized = sanitized.Replace(" ", "-");

            // Limit length to avoid potential path length issues
            int maxLength = 50;
            if (sanitized.Length > maxLength)
            {
                sanitized = sanitized.Substring(0, maxLength);
            }

            return sanitized.ToLower();
        }
        public string GenerateUniqueFileName(string originalFileName, string extension)
        {
            var filenamewithoutextention= Path.GetFileNameWithoutExtension(originalFileName);
            var filenamesubstring = filenamewithoutextention.Length > 10 ? filenamewithoutextention.Substring(0, 10) : filenamewithoutextention;
            var guid = Guid.NewGuid().ToString("N").Substring(0,8);
            return $"{filenamesubstring}_{guid}{extension}";
        }
        public async Task<string> savestreamimage(Stream imageStream,EntityTyp entityType,string entityName)
        {
            if (imageStream == null || imageStream.Length == 0)
            {
                throw new ArgumentException("No image stream provided");
            }

            // Create directory structure if it doesn't exist
            string entityTypeFolder = entityType.ToString() + "s"; // Add 's' to make it plural
            string sanitizedEntityName = SanitizeFileName(entityName);

            // Get the absolute path for the Images directory
            string baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            string imagesDirectory = Path.Combine(baseDirectory, "Images");

            // Create the complete directory path
            string directoryPath = Path.Combine(imagesDirectory, entityTypeFolder, sanitizedEntityName);

            // Ensure all directory levels exist
            Directory.CreateDirectory(directoryPath);

            // Create filename using GUID + extension
            string fileName = $"{Guid.NewGuid()}.jpg";
            string fullFilePath = Path.Combine(directoryPath, fileName);

            // Save the image to the file system
            using (var fileStream = new FileStream(fullFilePath, FileMode.Create))
            {
                imageStream.Position = 0; // Reset stream position
                await imageStream.CopyToAsync(fileStream);
            }

            // Return the relative path (from wwwroot) to be stored in the database
            return Path.Combine("Images", entityTypeFolder, sanitizedEntityName, fileName).Replace("\\", "/");
        }
        public async Task<bool> deleteimage(string urlimage)
        {
            if (urlimage==null||urlimage.Length==0)
            {
                throw new Exception("is null");
            }
            var fullPath = Path.Combine(_imagesFolder, urlimage);
            if (File.Exists(fullPath))
            {
                await Task.Run(() => File.Delete(fullPath));
                return true;
            }
            return false;
        }

        public async Task<string> UpdateImageAsync(IFormFile newImageFile, string oldImagePath,EntityTyp entityType, string name)
        {
            // Delete old image if it exists
            if (!string.IsNullOrEmpty(oldImagePath))
            {
                await deleteimage(oldImagePath);
            }

            // Save new image
            return await saveimageurl(newImageFile, entityType, name);
        }

        public string GetImageUrl(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                return null;
            }

            // Convert backslashes to forward slashes for URL format
            return relativePath.Replace("\\", "/");
        }
    }
}
