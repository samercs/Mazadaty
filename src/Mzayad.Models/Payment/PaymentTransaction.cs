using Mzayad.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mzayad.Models.Payment
{
    public abstract class PaymentTransaction : ModelBase
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentTransactionId { get; set; }
        
        [Required]
        public int OrderId { get; set; }
        
        [Required]
        public PaymentStatus Status { get; set; }

        public string RequestParams { get; set; }

        public virtual Order Order { get; set; }

        public abstract string GetTransactionReference();
    }
}