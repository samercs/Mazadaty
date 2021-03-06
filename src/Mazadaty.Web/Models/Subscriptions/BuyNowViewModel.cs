using System;
using Mazadaty.Models;

namespace Mazadaty.Web.Models.Subscriptions
{
    public class BuyNowViewModel
    {
        public Subscription Subscription { get; set; }
        public DateTime? CurrentExpirationUtc { get; set; }
        public int AvailableTokens { get; set; }

        public DateTime NewExpirationUtc
        {
            get
            {
                if (!CurrentExpirationUtc.HasValue)
                {
                    return DateTime.UtcNow.AddDays(Subscription.Duration);
                }

                return CurrentExpirationUtc.Value.AddDays(Subscription.Duration);
            }
        }

        public int AvailableTokensAfterPurchase
        {
            get { return AvailableTokens - Subscription.PriceTokens.GetValueOrDefault(0); }
        }
    }
}
