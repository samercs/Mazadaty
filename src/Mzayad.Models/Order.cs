using Mzayad.Models.Enum;
using OrangeJetpack.Base.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Mzayad.Models.Enums;

namespace Mzayad.Models
{
    public class Order : EntityBase
    {
        public int OrderId { get; set; }

        [StringLength(128)]
        public string UserId { get; set; }

        public int? AddressId { get; set; }

        [Required]
        public OrderType Type { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        public bool AllowPhoneSms { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal Total { get; set; }
        
        public PaymentMethod? PaymentMethod { get; set; }

        public DateTime? SubmittedUtc { get; set; }
        public DateTime? ShippedUtc { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("AddressId")]
        public virtual ShippingAddress Address { get; set; }

        public virtual ICollection<OrderItem> Items { get; set; }
        public virtual ICollection<OrderLog> Logs { get; set; }

        public void RecalculateTotal()
        {
            Subtotal = Items.Sum(i => i.ItemPrice * i.Quantity);
            Total = Subtotal + Shipping;
        }

        public bool IsSubscription
        {
            get { return Items.Any(i => i.SubscriptionId.HasValue); }
        }
    }
}
