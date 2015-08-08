using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using OrangeJetpack.Localization;

namespace Mzayad.Models
{
    [Table("UsersTrophies")]
    public class UserTrophy : ModelBase , ILocalizable
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserTrohyId { get; set; }

        [Required,StringLength(128)]
        public string UserId { get; set; }

        [Required]
        public int TrophyId { get; set; }

        [Required]
        public int XpAwarded { get; set; }


        public virtual Trophy Trophy { get; set; }

        public virtual ApplicationUser Users { get; set; }
    }
}
