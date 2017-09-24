using OrangeJetpack.Base.Data;
using System.ComponentModel.DataAnnotations;

namespace Mazadaty.Models
{
    public class SessionLog : EntityBase
    {
        public int SessionLogId { get; set; }

        [Required]
        public string UserId { get; set; }

        public string IP { get; set; }

        public string Browser { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
