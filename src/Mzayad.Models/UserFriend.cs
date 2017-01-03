using OrangeJetpack.Base.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mzayad.Models
{
    [Table("UsersFriends")]
    public class UserFriend : EntityBase
    {
        [Key, Column(Order = 1)]
        [Required, StringLength(128)]
        [ForeignKey("User")]
        public string UserId { get; set; }

        [Key, Column(Order = 2)]
        [Required, StringLength(128)]
        [ForeignKey("Friend")]
        public string FriendId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ApplicationUser Friend { get; set; }
    }
}
