using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace e_comerce_api.models
{
    public class AffiliateComession
    {
        [Key]
        public int CommissionId { get; set; }
        [ForeignKey("Affiliate")]
        public int AffiliateId { get; set; }
        [ForeignKey("order")]
        public int OrderId { get; set; }         
        public decimal OrderAmount { get; set; }  // قيمة الطلب
        public decimal CommissionRate { get; set; } // نسبة العمولة (مثلاً 5%)
        public decimal CommissionAmount { get; set; } // العمولة الفعلية المحسوبة
        public bool IsPaid { get; set; }          // هل العمولة اتدفعت للمروج ولا لسه؟
        public DateTime CreatedAt { get; set; }   // تاريخ تسجيل العمولة
        public DateTime? PaidAt { get; set; }
        public order order { get; set; }
        public Affiliate Affiliate { get; set; }
    }
}
