using Mazadaty.Models.Enum;
using OrangeJetpack.Base.Data;
using OrangeJetpack.Localization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mazadaty.Models
{
    public class Trophy : EntityBase, ILocalizable
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TrophyId { get; set; }

        [Required, Index(IsUnique = true)]
        public TrophyKey Key { get; set; }

        [Required, Localized]
        public string Name { get; set; }

        [Required, Localized, UIHint("TextArea")]
        public string Description { get; set; }

        [Required, DataType(DataType.Url)]
        public string IconUrl { get; set; }

        [Required]
        public int XpAward { get; set; }
    }
}
