using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Humanizer;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Core.ShoppingCart;
using Mzayad.Web.Models.Checkout;
using OrangeJetpack.Base.Web;

namespace Mzayad.Web.Controllers
{
    [LanguageRoutePrefix("checkout")]
    public class CheckoutController : ApplicationController
    {
        private readonly ProductService _productService;
        private readonly OrderService _orderService;
        private readonly AddressService _addressService;
        public CheckoutController(IAppServices appServices) : base(appServices)
        {
            _productService = new ProductService(DataContextFactory);
            _orderService = new OrderService(DataContextFactory);
            _addressService = new AddressService(DataContextFactory);
        }

        [Route("create-order")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateOrder(IList<CartItem> cartItems)
        {
            var cart = await UpdateAndValidateCart(cartItems);
            if (cart.HasErrors)
            {
                return CartError(cart);
            }

            if (!AuthService.IsAuthenticated())
            {
                return ReturnSignInView();
            }

            var user = await AuthService.CurrentUser();

            return await CreateOrderAndGetResult(user, cart);
        }

        private async Task<ShoppingCart> UpdateAndValidateCart(IEnumerable<CartItem> cartItems)
        {
            var cart = CartService.GetCart();
            CartService.ClearCart(cart);
            CartService.AddItems(cart, cartItems);
            return await CartService.ValidateCart(cart, _productService);
        }

        private ActionResult CartError(ShoppingCart cart)
        {
            var stateErrors = cart.Items
                .SelectMany(i => i.StateErrors)
                .Select(i => i.Humanize(LetterCasing.Title))
                .Distinct();

            var errorList = string.Join(", ", stateErrors);
            var errorMessage = $"We're sorry, but there was one or more problems with the items in your shopping cart and we've adjusted it to correct the problem. Prices may have changed since items were added to your cart or items aren't available at the quantity requested. [ERRORS: {errorList}]";

            SetStatusMessage(errorMessage, StatusMessageType.Warning);

            return RedirectToAction("Cart", "Shop", new { Language });
        }

        private ActionResult ReturnSignInView()
        {
            var viewModel = new CheckoutViewModel
            {
                Email = CookieService.Get(CookieKeys.LastSignInEmail),
                CheckoutMode = CheckoutMode.AsUser
            };

            return View("SignIn", viewModel);
        }

        private async Task<ActionResult> CreateOrderAndGetResult(ApplicationUser user, ShoppingCart cart)
        {
            user.Address = await _addressService.GetAddress(user.AddressId);
            var newOrder = await _orderService.CreateOrderForCart(cart.Items.Select(CartItem.Create).ToList(), user);
            return RedirectToAction("Shipping", "Orders", new {Language, newOrder.OrderId});
        }
    }
}