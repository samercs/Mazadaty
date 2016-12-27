using OrangeJetpack.Base.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mzayad.Models
{
    public class UserPrizeLog : EntityBase
    {
        [Required, Key, Column(Order = 1)]
        public int PrizeId { get; set; }
        [Required, Key, Column(Order = 2)]
        [StringLength(128)]
        public string UserId { get; set; }
        [Required, Key, Column(Order = 3)]
        public string Hash { get; set; }
        [DefaultValue(false)]
        public bool IsComplete { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual Prize Prize { get; set; }
    }
}