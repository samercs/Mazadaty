using Mzayad.Models;

namespace Mzayad.Web.Areas.Api.Models.Orders
{
    public class OrderItemModel
    {
        public int ItemId { get; set; }
        public decimal Prices { get; set; }
        public int Quntity { get; set; }
        public decimal TotalPrices { get; set; }
        public string Name { get; set; }

        public static OrderItemModel Create(OrderItem orderItem)
        {
            return new OrderItemModel
            {
                ItemId = orderItem.OrderItemId,
                Quntity = orderItem.Quantity,
                Prices = orderItem.ItemPrice,
                TotalPrices = orderItem.TotalPrice,
                Name = orderItem.Product?.Name
            };
        }

    }
}