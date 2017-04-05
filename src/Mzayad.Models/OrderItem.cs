using OrangeJetpack.Base.Data;
using OrangeJetpack.Localization;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mzayad.Models
{
    public class OrderItem : EntityBase, ILocalizable
    {
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public int? ProductId { get; set; }
        public int? SubscriptionId { get; set; }
        
        [Localized]
        public string Name { get; set; }
        public decimal ItemPrice { get; set; }
        public int Quantity { get; set; }
        
        public virtual Product Product { get; set; }
        public virtual Subscription Subscription { get; set; }
        public  virtual Order Order { get; set; }

        [NotMapped]
        public decimal TotalPrice => Quantity * ItemPrice;
    }
}
