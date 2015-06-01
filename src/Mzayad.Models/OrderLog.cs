using Mzayad.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mzayad.Models
{
    public class OrderLog : ModelBase
    {
        public int OrderLogId { get; set; }

        [Required]
        public int OrderId { get; set; }

        public string UserId { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        [Required, StringLength(15)]
        public string UserHostAddress { get; set; }

        public virtual Order Order { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
