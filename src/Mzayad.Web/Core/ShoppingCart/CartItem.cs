using System;
using System.Collections.Generic;
using Mzayad.Models;
using OrangeJetpack.Localization;

namespace Mzayad.Web.Core.ShoppingCart
{
    [Serializable]
    public class CartItem
    {
        public string Name { get; set; }
        public int ProductId { get; set; }
        public int AuctionId { get; set; }
        public string ImageUrl { get; set; }
        public int Quantity { get; set; }
        public decimal ItemPrice { get; set; }
        public DateTime AddedUtc { get; set; }
        public decimal TotalPrice => Quantity * ItemPrice;
        public IList<CartItemStateError> StateErrors { get; set; }

        public CartItem()
        {
            StateErrors = new List<CartItemStateError>();
        }

        public static OrderItem Create(CartItem cartItem)
        {
            return new OrderItem
            {
                ProductId = cartItem.ProductId,
                Quantity = cartItem.Quantity,
                ItemPrice = cartItem.ItemPrice,
                Name = new[]
                    {
                        new LocalizedContent("en", cartItem.Name),
                        new LocalizedContent("ar", cartItem.Name)
                    }.Serialize()
            };
        }
    }

    [Serializable]
    public enum CartItemStateError
    {
        ProductNotAvailable,
        InsufficientQuantity,
        UnitPriceChanged
    }
}