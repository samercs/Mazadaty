using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Localization;
using System;

namespace Mzayad.Models
{
    public class BuyNowTransactionModel
    {
        public int OrderId { get; set; }

        public string UserName { get; set; }

        public string ItemName { get; set; }

        public decimal Amount { get; set; }

        public DateTime OrderDate { get; set; }

        public static BuyNowTransactionModel Create(OrderItem order)
        {
            return new BuyNowTransactionModel
            {
                OrderId = order.Order.OrderId,
                UserName = NameFormatter.GetFullName(order.Order.User.FirstName, order.Order.User.LastName),
                ItemName = order.Product.Localize("en", i => i.Name).Name,
                Amount = order.ItemPrice,
                OrderDate = order.Order.CreatedUtc
            };
        }
    }
}
