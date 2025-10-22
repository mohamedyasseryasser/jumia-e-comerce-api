using System.ComponentModel.DataAnnotations;

namespace e_comerce_api.DTO
{
    public class useridandroledto
    {
        public string UserId { get; set; }
        public string role {  get; set; }
    }
    public class Userdto
    {
        public string userid {  get; set;}
        public string name { get; set; }
        public string email { get; set; }
        public string phone {  get; set; }
        public string role {  get; set; }
        [Required]
        public DateTime lastlogin { get; set; }
        [Required]
        public DateTime createdAt { get; set; }
        [Required]
        public DateTime updatedAt { get; set; }
        public bool? IsActive { get; set; }
        public List<addressresponsedto> addresses { get; set; }=new List<addressresponsedto>();
    }
    public class addressresponsedto
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public bool? IsDefault { get; set; }
        public string AddressName { get; set; }
    }
    public class UserStatusUpdateDto
    {
        [Required]
        public bool? IsActive { get; set; }
    }
}
