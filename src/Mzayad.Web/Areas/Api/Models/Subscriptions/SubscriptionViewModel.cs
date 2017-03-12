using Mzayad.Models;

namespace Mzayad.Web.Areas.Api.Models.Subscriptions
{
    public class SubscriptionViewModel
    {
        public int SubscriptionId { get; set; }
        public string Name { get; set; }
        public int? PriceTokens { get; set; }
        public decimal? PriceCurrency { get; set; }
        public int Duration { get; set; }
        public int? Quantity { get; set; }

        public static SubscriptionViewModel Create(Subscription subscription)
        {
            return new SubscriptionViewModel
            {
                SubscriptionId = subscription.SubscriptionId,
                PriceTokens = subscription.PriceTokens,
                Name = subscription.Name,
                PriceCurrency = subscription.PriceCurrency,
                Duration = subscription.Duration,
                Quantity = subscription.Quantity
            };
        }
    }
}