using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mzayad.Models
{
    public class WishList : ModelBase
    {
        public int WishListId { get; set; }
        [Required,StringLength(128)]
        public string UserId { get; set; }
        [Required]
        public string NameEntered { get; set; }
        [Required]
        public string NameNormalized { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

    }

    [NotMapped]
    public class WishListAdminModel
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }
}
