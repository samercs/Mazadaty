using OrangeJetpack.Base.Data;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Mazadaty.Models
{
    public class UserPrizeLog : EntityBase
    {
        public int UserPrizeLogId { get; set; }
        [Required]
        [StringLength(128)]
        public string UserId { get; set; }
        public int? PrizeId { get; set; }
        [DefaultValue(false)]
        public bool IsComplete { get; set; }
        public DateTime? CompleteDateTime { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Prize Prize { get; set; }
    }
}
