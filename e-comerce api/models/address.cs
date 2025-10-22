using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class address
    {
        [Key]
        public int AddressId { get; set; }
        [ForeignKey("user")]
        public string? UserId { get; set; }
        public applicationuser? user { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public bool? IsDefault { get; set; }
        public string AddressName { get; set; }
        public virtual ICollection<order> Orders { get; set; } = new List<order>();
    }
}
