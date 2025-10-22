using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.DTO
{
    public class CustomerResponsedto
    {
        public int Customerid { get; set; }
         public string? user_id { get; set; }
        public DateTime lastlogin { get; set; }
        [Required]
        public DateTime createdAt { get; set; }
        [Required]
        public DateTime updatedAt { get; set; }
        public bool? IsActive { get; set; }
        public string name {  get; set; }
        public string email {  get; set; }
        public string phone { get; set; }
        public List<addressresponsedto> addresses { get; set; } = new List<addressresponsedto>();
    }
    public class PagedResult<T>
    {
        public int TotalItems { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public List<T> Data { get; set; }
    }
    public class customerupdatedto {
        public int customer_id {  get; set; }
        public string user_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public List<addressresponsedto> addresses { get; set; } = new List<addressresponsedto>();

    }
}
