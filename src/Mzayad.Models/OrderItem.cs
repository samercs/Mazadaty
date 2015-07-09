﻿using System.ComponentModel.DataAnnotations.Schema;
using OrangeJetpack.Localization;

namespace Mzayad.Models
{
    public class OrderItem : ModelBase, ILocalizable
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

        [NotMapped]
        public decimal TotalPrice
        {
            get
            {
                return Quantity * ItemPrice;
            }
        }
    }
}
