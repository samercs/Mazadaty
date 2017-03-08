using Mzayad.Models;

namespace Mzayad.Web.Areas.Api.Models.Subscriptions
{
    public class SubscriptionViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Token { get; set; }
        public decimal? Prices { get; set; }
        public int Duration { get; set; }

        public static SubscriptionViewModel Create(Subscription subscription)
        {
            return new SubscriptionViewModel
            {
                Id = subscription.SubscriptionId,
                Token = subscription.PriceTokens,
                Name = subscription.Name,
                Prices = subscription.PriceCurrency,
                Duration = subscription.Duration
            };
        }
    }
}