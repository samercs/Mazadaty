using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mazadaty.Models.Payment
{
    public class KnetTransaction : PaymentTransaction
    {
        [Required, Index(IsUnique = true)]
        [StringLength(255)]
        public string PaymentId { get; set; }    
        
        [StringLength(255)]
        public string Result { get; set; }
        
        [StringLength(255)]
        public string AuthorizationNumber { get; set; }
        
        [StringLength(255)]
        public string ReferenceNumber { get; set; }
        
        [StringLength(255)]
        public string PostDate { get; set; }
        
        [StringLength(255)]
        public string TransactionId { get; set; }
        
        [StringLength(255)]
        public string TrackId { get; set; }

        public override string GetTransactionReference()
        {
            return ReferenceNumber;
        }
    }
}
