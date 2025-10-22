using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class admin
    {
        [Key]
        public int Adminid {  get; set; }
        [ForeignKey("user")]
        public string? user_id { get; set; }
        public applicationuser? user { get; set; }
        [Required]
        public string role {  get; set; }
        [Required]
        public string permission {  get; set; }
    }
}
