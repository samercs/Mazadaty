using Mazadaty.Core.Formatting;
using Mazadaty.Models;
using Mazadaty.Web.Resources;
using OrangeJetpack.Base.Core.Formatting;

namespace Mazadaty.Web.Extensions
{
    public static class SubscriptionExtensions
    {
        /// <summary>
        /// Gets a formatted string of a subscriptions price in KD and/or tokens.
        /// </summary>
        public static string FormattedPrice(this Subscription subscription)
        {
            if (!subscription.PriceCurrencyIsValid && !subscription.PriceTokensIsValid)
            {
                return "";
            }

            if (subscription.PriceCurrencyIsValid && !subscription.PriceTokensIsValid)
            {
                return CurrencyFormatter.Format(subscription.PriceCurrency);
            }

            if (!subscription.PriceCurrencyIsValid && subscription.PriceTokensIsValid)
            {
                return StringFormatter.ObjectFormat(Global.SubscriptionPriceTokens, new { subscription.PriceTokens });
            }

            return StringFormatter.ObjectFormat(Global.SubscriptionPriceKdAndTokens, new
            {
                PriceCurrency = CurrencyFormatter.Format(subscription.PriceCurrency),
                subscription.PriceTokens
            });
        }
    }
}
