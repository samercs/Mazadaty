using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrangeJetpack.Localization;

namespace Mzayad.Models
{
    public class Sponsor : ModelBase, ILocalizable
    {
        public int SponsorId { get; set; }

        [Required, Localized]
        public string Name { get; set; }
    }
}
