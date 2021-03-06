using OrangeJetpack.Base.Data;
using OrangeJetpack.Localization;
using System.ComponentModel.DataAnnotations;

namespace Mazadaty.Models
{
    public class Sponsor : EntityBase, ILocalizable
    {
        public int SponsorId { get; set; }

        [Required, Localized]
        public string Name { get; set; }
    }
}
