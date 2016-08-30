using OrangeJetpack.Base.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mzayad.Models
{
    public class CategoryNotification : EntityBase
    {
        public int CategoryNotificationId { get; set; }
        [StringLength(128)]
        public string UserId { get; set; }
        public int CategoryId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
