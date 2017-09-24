using OrangeJetpack.Base.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mazadaty.Models
{
    public class SubscriptionLog : EntityBase
    {
        public int SubscriptionLogId { get; set; }
        
        [Required, StringLength(128)]
        public string UserId { get; set; }
        
        [Required, StringLength(128)]
        public string ModifiedByUserId { get; set; }
        
        public DateTime? OriginalSubscriptionUtc { get; set; }
        
        [Required]
        public DateTime ModifiedSubscriptionUtc { get; set; }
        
        [Required, StringLength(15)]
        public string UserHostAddress { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("ModifiedByUserId")]
        public virtual ApplicationUser ModifiedByUser { get; set; }

        [NotMapped]
        public int DaysAdded
        {
            get
            {
                if (!OriginalSubscriptionUtc.HasValue)
                {
                    OriginalSubscriptionUtc = DateTime.Today;
                }

                return ModifiedSubscriptionUtc.Subtract(OriginalSubscriptionUtc.Value).Days;

            }
        }
    }
}
