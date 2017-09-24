using Mazadaty.Models.Enum;
using OrangeJetpack.Base.Data;
using OrangeJetpack.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mazadaty.Models
{
    [Serializable]
    public class Subscription : EntityBase, ILocalizable, IValidatableObject
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

        public bool PriceCurrencyIsValid => PriceCurrency.HasValue && PriceCurrency.Value > 0;

        public bool PriceTokensIsValid => PriceTokens.HasValue && PriceTokens.Value > 0;

        public bool IsSoldOut => Quantity.HasValue && Quantity.Value <= 0;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!(PriceCurrencyIsValid || PriceTokensIsValid))
            {
                yield return new ValidationResult("Purchase price cannot be null or zero.", new[] { "PriceCurrency", "PriceTokens" });
            }
        }
    }
}
