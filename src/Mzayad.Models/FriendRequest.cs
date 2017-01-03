using Mzayad.Models.Enums;
using OrangeJetpack.Base.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mzayad.Models
{
    public class FriendRequest : EntityBase
    {
        [Key]
        public int FriendRequestId { get; set; }

        [Required]
        public FriendRequestStatus Status { get; set; }

        [Required, StringLength(128)]
        public string UserId { get; set; }

        [Required, StringLength(128)]
        [ForeignKey("Friend")]
        public string FriendId { get; set; }


        public virtual ApplicationUser Friend { get; set; }

    }
}
