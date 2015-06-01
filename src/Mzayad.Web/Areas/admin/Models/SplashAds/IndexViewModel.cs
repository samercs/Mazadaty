using Mzayad.Models;
using System.Collections.Generic;

namespace Mzayad.Web.Areas.Admin.Models.SplashAds
{
    public class IndexViewModel
    {
        public IReadOnlyCollection<SplashAd> SplashAds { get; set; } 
    }
}
