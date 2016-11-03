using OrangeJetpack.Base.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mzayad.Models
{
    public class TokenLog : EntityBase
    {
        public int TokenLogId { get; set; }

        [Required, StringLength(128)]
        public string UserId { get; set; }
        
        [Required, StringLength(128)]
        public string ModifiedByUserId { get; set; }

        public int? OriginalTokenAmount { get; set; }

        [Required]
        public int ModifiedTokenAmount { get; set; }

        [Required, StringLength(15)]
        public string UserHostAddress { get; set; }

        public string Usage { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("ModifiedByUserId")]
        public virtual ApplicationUser ModifiedByUser { get; set; }

        [NotMapped]
        public int TokensAdded => ModifiedTokenAmount - OriginalTokenAmount.GetValueOrDefault(0);
    }
}
