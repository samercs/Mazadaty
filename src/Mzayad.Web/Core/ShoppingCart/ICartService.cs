using System.Collections.Generic;
using System.Threading.Tasks;
using Mzayad.Services;

namespace Mzayad.Web.Core.ShoppingCart
{
    public interface ICartService
    {
        void InheritAnonymousCart(string userId);
        void ClearCart();
        ShoppingCart ClearCart(ShoppingCart shoppingCart);
        ShoppingCart GetCart();
        ShoppingCart SaveCart(ShoppingCart shoppingCart);
        ShoppingCart AddItem(ShoppingCart shoppingCart, CartItem cartItem);
        ShoppingCart AddItems(ShoppingCart shoppingCart, IEnumerable<CartItem> cartItems);
        Task<ShoppingCart> ValidateCart(ShoppingCart shoppingCart, ProductService productService);
    }
}