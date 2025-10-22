using System.ComponentModel.DataAnnotations;

namespace e_comerce_api.DTO
{
    public class TokenResult
    {
        public string Token { get; set; }
        public DateTime ExpireAt { get; set; }
        public string RefreshToken { get; set; }      
        public DateTime RefreshTokenExpireAt { get; set; }

    }
    public class AdminRegisterDto:registerdto
    {
        public string permission {  get; set; }
    }
    public class CustomerRegisterDto : registerdto
    {

    }
    public class SellerRegisterDto:registerdto
    {
        public string businessName { get; set; }
        [Required]
        public string CustomerServicesPhone { get; set; }
        [Required]
        public string businessDesription { get; set; }
        [Required]
        public string logo { get; set; }
        public float rate { get; set; }
        [Required]
        public bool is_verfied { get; set; }
        public DateTime verfiedAt { get; set; }
    }
    public class registerdto
    {
        [Required]
        public string username {  get; set; }
        [Required]
        public string email {  get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("password")]
        public string confirmpassword { get; set; }
        public string phone {  get; set; }
        public addressdto address {  get; set; }
    }
    public class addressdto
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public bool? IsDefault { get; set; }
        public string AddressName { get; set; }
    }
    public class logindto
    {
        [Required]
        public string username { get; set; }
        [Required]
        public string password { get; set; }
    }
    public class roledto
    {
        public string role {  get; set; }
    }
    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; }
    }
    public class TokenResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
    public class ChangePasswordDto
    {
        public string email {  get; set; }
        public string newpassword {  get; set; }
        public string oldpassword { get; set; }
    }
}
