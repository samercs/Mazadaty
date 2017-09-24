using Mazadaty.Models.Enum;
using OrangeJetpack.Base.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mazadaty.Models
{
    public class OrderLog : EntityBase
    {
        public int OrderLogId { get; set; }

        [Required]
        public int OrderId { get; set; }

        public string UserId { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        public virtual Order Order { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
