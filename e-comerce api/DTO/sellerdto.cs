namespace e_comerce_api.DTO
{
    public class SellerResponseDto
    {
        public int Sellerid { get; set; }
        public string user_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string businessName { get; set; }
        public string businessDesription { get; set; }
        public string CustomerServicesPhone { get; set; }
        public string logo { get; set; }
        public float rate { get; set; }
        public bool is_verfied { get; set; }
        public DateTime verfiedAt { get; set; }
        public DateTime createdAt { get; set; }
        public DateTime updatedAt { get; set; }
        public DateTime lastlogin { get; set; }
        public bool? IsActive { get; set; }
    }

    public class SellerUpdateDto
    {
        public int Sellerid { get; set; }
        public string user_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string businessName { get; set; }
        public string businessDesription { get; set; }
        public string CustomerServicesPhone { get; set; }
        public string logo { get; set; }
    }

}
