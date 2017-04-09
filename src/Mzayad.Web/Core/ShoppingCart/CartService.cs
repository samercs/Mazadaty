using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mzayad.Services;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Base.Web.Caching;

namespace Mzayad.Web.Core.ShoppingCart
{
    public class CartService : ICartService
    {
        private readonly ICacheService _cacheService;
        private string _cacheKey;

        public CartService(IHttpContextService httpContextService, ICacheService cacheService)
        {
            _cacheService = cacheService;

            var userId = httpContextService.IsAuthenticated()
                ? httpContextService.GetUserId()
                : httpContextService.GetAnonymousId();

            _cacheKey = GetCacheKey(userId);
        }

        private static string GetCacheKey(string userId)
        {
            return string.Format(CacheKeys.ShoppingCart, userId.ToLowerInvariant());
        }

        /// <summary>
        /// Gets an anonymous user's cached cart and associates it with an authenticated user.
        /// </summary>
        public void InheritAnonymousCart(string userId)
        {
            var shoppingCart = GetCart();

            ClearCart();

            _cacheKey = GetCacheKey(userId);
            SaveCart(shoppingCart);
        }

        public void ClearCart()
        {
            SaveCart(new ShoppingCart());
        }

        public ShoppingCart ClearCart(ShoppingCart shoppingCart)
        {
            shoppingCart.Items.Clear();
            return SaveCart(shoppingCart);
        }

        public ShoppingCart GetCart()
        {
            var shoppingCart = _cacheService.Get<ShoppingCart>(_cacheKey) ?? new ShoppingCart();
            shoppingCart.Items = shoppingCart.Items.OrderBy(i => i.AddedUtc).ToList();

            return shoppingCart;
        }

        public ShoppingCart SaveCart(ShoppingCart shoppingCart)
        {
            _cacheService.Set(_cacheKey, shoppingCart, TimeSpan.FromDays(30));

            return shoppingCart;
        }

        public ShoppingCart AddItem(ShoppingCart shoppingCart, CartItem cartItem)
        {
            cartItem.AddedUtc = DateTime.UtcNow;

            var existingItem = shoppingCart.Items.FirstOrDefault(i => i.ProductId == cartItem.ProductId);
            if (existingItem != null)
            {
                return shoppingCart;
            }

            if (cartItem.Quantity > 0)
            {
                shoppingCart.Items.Add(cartItem);
            }

            return shoppingCart;
        }

        public ShoppingCart AddItems(ShoppingCart shoppingCart, IEnumerable<CartItem> cartItems)
        {
            foreach (var cartItem in cartItems.Where(i => i.Quantity > 0))
            {
                AddItem(shoppingCart, cartItem);
            }

            return shoppingCart;
        }

        public async Task<ShoppingCart> ValidateCart(ShoppingCart shoppingCart, ProductService productService)
        {
            shoppingCart.HasErrors = false;

            if (!shoppingCart.Items.Any())
            {
                shoppingCart.HasErrors = true;
                return shoppingCart;
            }

            var products = (await productService.GetProductsByIdList(shoppingCart.Items.Select(i => i.ProductId))).ToList();
            foreach (var cartItem in shoppingCart.Items.ToList())
            {
                var product = products.SingleOrDefault(i => i.ProductId == cartItem.ProductId);
                if (product == null)
                {
                    shoppingCart.Items.Remove(cartItem);
                    shoppingCart.HasErrors = true;
                    continue;
                }

                if (product.Quantity == 0)
                {
                    cartItem.Quantity = 0;
                    cartItem.StateErrors.Add(CartItemStateError.ProductNotAvailable);
                    shoppingCart.HasErrors = true;
                    continue;
                }
                if (cartItem.Quantity == 0)
                {
                    cartItem.StateErrors.Add(CartItemStateError.ProductNotAvailable);
                    shoppingCart.HasErrors = true;
                    continue;
                }
            }

            return SaveCart(shoppingCart);
        }
    }
}