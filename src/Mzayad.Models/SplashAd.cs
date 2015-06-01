using System.ComponentModel.DataAnnotations;

namespace Mzayad.Models
{
    public class SplashAd : ModelBase
    {
        public int SplashAdId { get; set; }

        [DataType(DataType.Url)]
        public string Url { get; set; }

        [Required]
        public int Weight { get; set; }

        public double SortOrder { get; set; }
    }
}
