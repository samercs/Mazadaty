using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrangeJetpack.Base.Data;

namespace Mzayad.Models
{
    public class UserAvatar: EntityBase
    {
        [Key, Required, StringLength(128), Column(Order = 1)]
        public string UserId { get; set; }
        [Key, Required, Column(Order = 2)]
        public int AvatarId { get; set; }

        public ApplicationUser User { get; set; }
        public Avatar Avatar { get; set; }

    }
}