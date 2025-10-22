using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class UserCopon
    {
        [Key]
        public int UserCoponId { get; set; }
        public bool? IsUsed { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? UsedAt { get; set; }
        [ForeignKey("copon")]
        public int copon_id {  get; set; }
        public Copons copon { get; set; }
        [ForeignKey("customer")]
        public int customer_id {  get; set; }
        public customer customer { get; set; }
    }
}
