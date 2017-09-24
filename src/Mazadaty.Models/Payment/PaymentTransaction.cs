using Mazadaty.Models.Enum;
using OrangeJetpack.Base.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mazadaty.Models.Payment
{
    public abstract class PaymentTransaction : EntityBase
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
