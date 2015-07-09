using Mzayad.Core.Formatting;
using Mzayad.Models;
using Mzayad.Web.Resources;
using OrangeJetpack.Base.Core.Formatting;

namespace Mzayad.Web.Extensions
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
                return StringFormatter.ObjectFormat(Global.SubscriptionPriceTokens, new { subscription });
            }

            return StringFormatter.ObjectFormat(Global.SubscriptionPriceKdAndTokens, new
            {
                PriceCurrency = CurrencyFormatter.Format(subscription.PriceCurrency),
                subscription.PriceTokens
            });
        }
    }
}
