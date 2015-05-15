using Mzayad.Models.Enum;
using OrangeJetpack.Localization;
using System;
using System.ComponentModel.DataAnnotations;

namespace Mzayad.Models
{
    [Serializable]
    public class Subscription : ModelBase , ILocalizable
    {
        public int SubscriptionId { get; set; }
        [Required, Localized]
        public string Name { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime? ExpirationUtc { get; set; }
        public int? Quantity { get; set; }     
    }
}
