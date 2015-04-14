using Mzayad.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mzayad.Models
{
    public class UserProfile : ModelBase
    {
        public int UserProfileId { get; set; }
        [Required,StringLength(128)]
        public string UserId { get; set; }
        [Required]
        public UserProfileStatus Status { get; set; }

        [Required, Index("IX_ProfileUrl", IsUnique = true),StringLength(255)]
        public string ProfileUrl { get; set; }
        public int? AvatarId { get; set; }
                
        [ForeignKey("AvatarId")] 
        public virtual Avatar Avatar { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public static string GenerateProfileUrl(string username)
        {
            return string.Format("https://www.mzayad.com/profiles/{0}", username.ToLowerInvariant());
        }
    }
}
