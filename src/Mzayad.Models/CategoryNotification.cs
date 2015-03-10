using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Mzayad.Models
{
    public class CategoryNotification : ModelBase
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
