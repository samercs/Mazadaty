using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Core.ShoppingCart;
using Mzayad.Web.Resources;
using OrangeJetpack.Base.Web;
using OrangeJetpack.Localization;

namespace Mzayad.Web.Controllers
{
    [LanguageRoutePrefix("shop")]
    public class ShopController : ApplicationController
    {
        private readonly ProductService _productService;
        private readonly AuctionService _auctionService;

        public ShopController(IAppServices appServices) : base(appServices)
        {
            _productService = new ProductService(DataContextFactory);
            _auctionService = new AuctionService(DataContextFactory, appServices.QueueService);
        }

        [Route("add-to-cart")]
        public async Task<ActionResult> AddToCart(int auctionId)
        {
            var auction = await _auctionService.GetAuctionById(auctionId);
            if (auction == null)
            {
                throw new ArgumentException("Cannot find auction for auctionId = " + auctionId);
            }
            var isNew = await SaveItemToCart(auction);
            if (isNew)
            {
                SetStatusMessage(string.Format(Global.ProductAddedToCartAcknowledgement,
                    auction.Product.Localize(Language, LocalizationDepth.OneLevel).Name));
            }
            else
            {
                SetStatusMessage(string.Format(Global.ProductExistInCart,
                    auction.Product.Localize(Language, LocalizationDepth.OneLevel).Name), StatusMessageType.Warning);
            }

            return RedirectToAction("Index", "Home");
        }

        [Route("cart")]
        public async Task<ActionResult> Cart()
        {
            var shoppingCart = CartService.GetCart();
            await CartService.ValidateCart(shoppingCart, _productService);
            CartService.SaveCart(shoppingCart);

            //if (ValidateMinimumOrderAmountWarning(shoppingCart))
            //{
            //    var message = StringFormatter.ObjectFormat(
            //        Global.MinimumOrderAmountWarningMessage,
            //        new { MinimumOrderAmount = CurrencyFormatter.Format(AppSettings.MinimumOrderAmount, CurrencyConverter) });
            //    SetStatusMessage(message, StatusMessageType.Warning);
            //}

            return View(shoppingCart);
        }

        private async Task<bool> SaveItemToCart(Auction auction)
        {
            var product = await _productService.GetProduct(auction.ProductId);
            if (product == null)
            {
                throw new ArgumentException("Cannot find products for productId = " + auction.ProductId);
            }
            product = product.Localize(Language, LocalizationDepth.OneLevel);
            var shoppingCart = CartService.GetCart();
            var existingItem = shoppingCart.Items.FirstOrDefault(i => i.ProductId == product.ProductId);
            if (existingItem != null)
            {
                return false;
            }
            var cartItem = new CartItem
            {
                Name = product.Name,
                ProductId = product.ProductId,
                ImageUrl = product.MainImage().ImageSmUrl,
                ItemPrice = auction.BuyNowPrice ?? 0,
                Quantity = 1,
                AuctionId = auction.AuctionId
            };

            CartService.AddItem(shoppingCart, cartItem);
            CartService.SaveCart(shoppingCart);
            return true;
        }

        [Route("remove-from-cart"), HttpPost]
        public async Task<JsonResult> RemoveItemFormCart(int productId)
        {
            var shoppingCart = CartService.GetCart();
            var removedItem = shoppingCart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (removedItem != null)
            {
                shoppingCart.Items.Remove(removedItem);
                await CartService.ValidateCart(shoppingCart, _productService);
                CartService.SaveCart(shoppingCart);
            }

            return Json("Success");
        }


    }
}