using System;
using Mzayad.Models;

namespace Mzayad.Web.Models.Subscriptions
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
    }
}
