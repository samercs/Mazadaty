using Mazadaty.Models;

namespace Mazadaty.Web.Areas.Api.Models.Ads
{
    public class AdsViewModel
    {
        public string ImageUrl { get; set; }
        public int MobileAdId { get; set; }
        public int Weight { get; set; }

        public static AdsViewModel Create(SplashAd ad)
        {
            return new AdsViewModel
            {
                ImageUrl = ad.Url,
                Weight = ad.Weight,
                MobileAdId = ad.SplashAdId
            };
        }
    }
}
