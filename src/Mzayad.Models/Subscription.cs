using Mzayad.Models.Enum;
using OrangeJetpack.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mzayad.Models
{
    [Serializable]
    public class Subscription : ModelBase , ILocalizable, IValidatableObject
    {
        public int SubscriptionId { get; set; }
        [Required, Localized]
        public string Name { get; set; }
        public SubscriptionStatus Status { get; set; }
        public int Duration { get; set; }
        public DateTime? ExpirationUtc { get; set; }
        public int? Quantity { get; set; }

        public decimal? PriceCurrency { get; set; }
        public int? PriceTokens { get; set; }

        public double SortOrder { get; set; }

        public bool PriceCurrencyIsValid
        {
            get { return PriceCurrency.HasValue && PriceCurrency.Value > 0; }
        }

        public bool PriceTokensIsValid
        {
            get { return PriceTokens.HasValue && PriceTokens.Value > 0; }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!PriceCurrencyIsValid || !PriceTokensIsValid)
            {
                yield return new ValidationResult("Purchase price cannot be null or zero.", new[] { "PriceCurrency", "PriceTokens" });
            }
        }
    }
}
