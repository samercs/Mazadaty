using OrangeJetpack.Base.Data;
using System.ComponentModel.DataAnnotations;

namespace Mazadaty.Models
{
    public class SplashAd : EntityBase
    {
        public int SplashAdId { get; set; }

        [DataType(DataType.Url)]
        public string Url { get; set; }

        [Required]
        public int Weight { get; set; }

        public double SortOrder { get; set; }
    }
}
