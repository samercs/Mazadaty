using System;
using System.Linq;
using Mazadaty.Models;
using Mazadaty.Models.Enum;
using Mazadaty.Models.Payment;
using Mazadaty.Services;
using Mazadaty.Services.Payment;
using Mazadaty.Web.Core.Attributes;
using Mazadaty.Web.Core.Services;
using Mazadaty.Web.Models.Order;
using Mazadaty.Web.Models.Shared;
using Mazadaty.Web.Resources;
using OrangeJetpack.Base.Web;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mazadaty.Models.Enums;
using OrangeJetpack.Base.Core.Formatting;
using DateTimeFormatter = Mazadaty.Core.Formatting.DateTimeFormatter;
using DetailsViewModel = Mazadaty.Web.Models.Order.DetailsViewModel;

namespace Mazadaty.Web.Controllers
{
    [LanguageRoutePrefix("orders"), Authorize]
    public class OrdersController : ApplicationController
    {
        private readonly AuctionService _auctionService;
        private readonly OrderService _orderService;
        private readonly KnetService _knetService;
        private readonly AddressService _addressService;

        public OrdersController(IAppServices appServices) : base(appServices)
        {
            _auctionService = new AuctionService(DataContextFactory, appServices.QueueService);
            _orderService = new OrderService(DataContextFactory);
            _knetService = new KnetService(DataContextFactory);
            _addressService = new AddressService(DataContextFactory);
        }

        [Route("buy-now/{auctionId:int}")]
        public async Task<ActionResult> BuyNow(int auctionId)
        {
            var auction = await _auctionService.GetAuction(auctionId, Language);
            if (auction == null)
            {
                return HttpNotFound();
            }


            var auctionCloseTime = auction.ClosedUtc ?? DateTime.UtcNow;
            var closeHoure = DateTime.UtcNow.Subtract(auctionCloseTime).TotalHours;
            if (closeHoure <= 2)
            {
                var user = await AuthService.CurrentUser();
                if (user == null)
                {
                    SetStatusMessage(string.Format(Global.AuctionBuyNowWarnning, DateTimeFormatter.ToTimeOnly(auctionCloseTime.AddHours(3), DateTimeFormatter.Format.Full)), StatusMessageType.Warning);
                    return View("Blank");
                }
                if (user.Subscription != UserSubscriptionStatus.Active)
                {
                    SetStatusMessage(string.Format(Global.AuctionBuyNowWarnning, DateTimeFormatter.ToTimeOnly(auctionCloseTime.AddHours(3), DateTimeFormatter.Format.Full)), StatusMessageType.Warning);
                    return View("Blank");
                }
            }

            return View(auction);
        }

        [Route("buy-now/{auctionId:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> BuyNow(int auctionId, string action, FormCollection formCollection)
        {
            var auction = await _auctionService.GetAuction(auctionId, Language);
            if (auction == null)
            {
                return HttpNotFound();
            }
            if (!auction.BuyNowAvailable())
            {
                SetStatusMessage(Global.OutOfStockErrorMessage, StatusMessageType.Error);
                return RedirectToAction("BuyNow", new { auction.AuctionId });
            }
            if (action.Equals("1"))// check out
            {
                var user = await AuthService.CurrentUser();
                user.Address = await _addressService.GetAddress(user.AddressId);
                var order = await _orderService.CreateOrderForBuyNow(auction, user);
                return RedirectToAction("Shipping", new { order.OrderId });
            }
            return RedirectToAction("AddToCart", "Shop", new { auctionId });
        }

        [Route("auction/{orderId:int}")]
        public async Task<ActionResult> Auction(int orderId)
        {
            var order = await _orderService.GetById(orderId);
            if (order == null)
            {
                return HttpNotFound();
            }

            var user = await AuthService.CurrentUser();

            SetStatusMessage(StringFormatter.ObjectFormat(Global.AuctionWinMessage, new { user.FirstName }));

            return Shipping(order);
        }

        [Route("shipping/{orderId:int}")]
        public async Task<ActionResult> Shipping(int orderId)
        {
            var order = await _orderService.GetById(orderId);
            if (order == null)
            {
                return HttpNotFound();
            }

            return Shipping(order);
        }

        private ActionResult Shipping(Order order)
        {
            var model = new ShippingAddressViewModel
            {
                Order = order,
                AddressViewModel = new AddressViewModel(order.Address),
                ShippingAddress = order.Address,
            }.Hydrate();

            return View("Shipping", model);
        }

        [Route("shipping/{orderId:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Shipping(int orderId, ShippingAddressViewModel model)
        {
            var user = await AuthService.CurrentUser();

            var order = await _orderService.GetById(orderId);
            if (order == null)
            {
                return HttpNotFound();
            }

            order.Address.Name = model.ShippingAddress.Name;
            order.Address.PhoneCountryCode = model.PhoneNumberViewModel.PhoneCountryCode;
            order.Address.PhoneLocalNumber = model.PhoneNumberViewModel.PhoneLocalNumber;
            order.Address.CountryCode = model.AddressViewModel.CountryCode;
            order.Address.CityArea = model.AddressViewModel.CityArea;
            order.Address.AddressLine1 = model.AddressViewModel.AddressLine1;
            order.Address.AddressLine2 = model.AddressViewModel.AddressLine2;
            order.Address.AddressLine3 = model.AddressViewModel.AddressLine3;
            order.Address.AddressLine4 = model.AddressViewModel.AddressLine4;
            order.Address.PostalCode = model.AddressViewModel.PostalCode;
            order.Address.StateProvince = model.AddressViewModel.StateProvince;
            order.Shipping = OrderService.CalculateShipping(order, user);

            order.RecalculateTotal();

            await _orderService.Update(order);

            return RedirectToAction("Summary", new { orderId });
        }

        [Route("summary/{orderId:int}")]
        public async Task<ActionResult> Summary(int orderId)
        {
            var order = await _orderService.GetById(orderId);
            if (order == null)
            {
                return HttpNotFound();
            }

            order.RecalculateTotal();

            var model = new OrderSummaryViewModel
            {
                Order = order,
                Language = Language
            };

            return View(model);
        }

        [Route("submit/{orderId:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Submit(int orderId)
        {
            var order = await _orderService.GetById(orderId);
            if (order == null)
            {
                return HttpNotFound();
            }

            order = await _orderService.SaveShippingAndPayment(order, PaymentMethod.Knet);

            var knetService = new KnetService(DataContextFactory);

            var result = await knetService.InitTransaction(order, AuthService.CurrentUserId(), AuthService.UserHostAddress());

            return Redirect(result.RedirectUrl);
        }

        [Route("success/{orderId:int}")]
        public async Task<ActionResult> Success(int orderId, string paymentId = null, string redirectUrl = null)
        {
            var order = await _orderService.GetById(orderId);
            if (order == null)
            {
                return HttpNotFound();
            }

            KnetTransaction knetTransaction = null;

            if (!string.IsNullOrEmpty(paymentId))
            {
                knetTransaction = await _knetService.GetTransaction(paymentId);
            }

            var viewModel = new DetailsViewModel
            {
                Order = OrderViewModel.Create(order),
                KnetTransaction = knetTransaction,
                RedirectUrl = redirectUrl
            };

            var shoppingCart = CartService.GetCart();
            CartService.ClearCart(shoppingCart);

            return View(viewModel);
        }
    }
}
