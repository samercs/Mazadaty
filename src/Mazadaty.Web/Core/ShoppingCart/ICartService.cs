using System.Collections.Generic;
using System.Threading.Tasks;
using Mazadaty.Services;

namespace Mazadaty.Web.Core.ShoppingCart
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
