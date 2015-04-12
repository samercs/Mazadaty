using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Models.Enum;

namespace Mzayad.Models
{
    public class UserProfile : ModelBase
    {
        public int UserProfileId { get; set; }
        [Required,StringLength(128)]
        public string UserId { get; set; }
        [Required]
        public UserProfileStatus Status { get; set; }
        [Required, StringLength(20), Index("IX_Gamertag", IsUnique = true)]
        public string Gamertag { get; set; }
        [Required, Index("IX_ProfileUrl", IsUnique = true),StringLength(255)]
        public string ProfileUrl { get; set; }
        public int? AvatarId { get; set; }
        
        
        [ForeignKey("AvatarId")] 
        public virtual Avatar Avatar { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

    }
}
