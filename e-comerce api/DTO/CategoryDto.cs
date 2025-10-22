using e_comerce_api.models;
using System.Text.Json.Serialization;

namespace e_comerce_api.DTO
{
    public class CategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public string images { get; set; }
        [JsonIgnore]
        public IFormFile ImageFile { get; set; }

    }
    public class categoryresponse:CategoryDto
    {
        public int categoryid {  get; set; }
    }
    public class updatecatresponsedto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }
        public string images { get; set; }
      //  [JsonIgnore]
        public IFormFile ImageFile { get; set; }
        public int categoryid { get; set; }

    }
}
