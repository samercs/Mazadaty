using System;

namespace Mazadaty.Web.Models.Shared
{
    [Serializable]
    public class SubscriptionExpiration
    {
        public DateTime? SubscriptionUtc { get; set; }

        public SubscriptionExpiration(DateTime? subscriptionUtc)
        {
            SubscriptionUtc = subscriptionUtc;
        }
    }
}
