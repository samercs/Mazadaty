using Mzayad.Models;

namespace Mzayad.Web.Models.Subscriptions
{
    public class BuyNowViewModel
    {
        public Subscription Subscription { get; set; }
        public int AvailableTokens { get; set; }
    }
}
