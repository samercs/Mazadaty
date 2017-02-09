using Mzayad.Models;
using Mzayad.Models.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mzayad.Web.Areas.Api.Models.Orders
{
    public class OrderModel
    {
        public int OrderId { get; set; }

        [Required]
        public int AuctionId { get; set; }

        public OrderType Type { get; set; }

        public ShippingAddress ShippingAddress { get; set; }

        public IReadOnlyCollection<OrderItem> Items { get; set; }

    }
}