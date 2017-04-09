using System;
using System.Collections.Generic;
using System.Linq;

namespace Mzayad.Web.Core.ShoppingCart
{
    [Serializable]
    public class ShoppingCart
    {
        public IList<CartItem> Items { get; set; }
        public bool HasErrors { get; set; }

        public decimal TotalPrice
        {
            get { return Items.Sum(i => i.TotalPrice); }
        }

        public ShoppingCart()
        {
            Items = new List<CartItem>();
            HasErrors = false;
        }
    }
}