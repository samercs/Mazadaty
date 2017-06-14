using Humanizer;
using Mzayad.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mzayad.Web.Areas.Api.Models.Orders
{
    public class OrderDetailModel
    {
        public int OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Status { get; set; }
        public decimal Total { get; set; }
        public string PaymentType { get; set; }
        public IEnumerable<OrderItemModel> Items { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Shipping { get; set; }

        public static OrderDetailModel Create(Order order)
        {
            return new OrderDetailModel
            {
                OrderId = order.OrderId,
                Status = order.Status.Humanize(),
                OrderDate = order.SubmittedUtc,
                PaymentType = order.PaymentMethod?.Humanize(),
                Shipping = order.Shipping,
                SubTotal = order.Subtotal,
                Total = order.Total,
                Items = order.Items.Select(OrderItemModel.Create)
            };
        }

    }
}