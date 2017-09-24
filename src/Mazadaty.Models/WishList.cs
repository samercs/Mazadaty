using OrangeJetpack.Base.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mazadaty.Models
{
    public class WishList : EntityBase
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
}
