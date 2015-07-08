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
        public DateTime? ExpirationUtc { get; set; }
        public int? Quantity { get; set; }

        public decimal? PurchasePrice { get; set; }
        public int? PurchaseTokens { get; set; }

        public double SortOrder { get; set; }
        
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PurchasePrice.GetValueOrDefault(0) <= 0 && PurchaseTokens.GetValueOrDefault(0) <= 0)
            {
                yield return new ValidationResult("", new[] { "PurchasePrice", "PurchaseTokens" });
            }
        }
    }
}
