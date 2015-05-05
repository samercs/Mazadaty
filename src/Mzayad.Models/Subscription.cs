using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Models.Enum;
using OrangeJetpack.Localization;

namespace Mzayad.Models
{
    [Serializable]
    public class Subscription : ModelBase , ILocalizable
    {
        public int SubscriptionId { get; set; }
        public string Name { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime ExpirationUtc { get; set; }
        public int? Quantity { get; set; }
        
    }
}
