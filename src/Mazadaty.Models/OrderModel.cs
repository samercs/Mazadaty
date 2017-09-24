using System;

namespace Mazadaty.Models
{
    public class OrderModel
    {
        public int OrderId { get; set; }

        public ShippingAddress Address { get; set; }

        public decimal Total { get; set; }

        public DateTime? SubmittedUtc { get; set; }

        public DateTime? ShippedUtc { get; set; }

        public static OrderModel Create(Order order)
        {
            return new OrderModel
            {
                OrderId = order.OrderId,
                Address = order.Address,
                Total = order.Total,
                SubmittedUtc = order.SubmittedUtc,
                ShippedUtc = order.ShippedUtc
            };
        }
    }
}
