using Mzayad.Models.Enum;
using Mzayad.Models.Payment;
using Mzayad.Services;
using Mzayad.Services.Payment;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.Order;
using Mzayad.Web.Models.Shared;
using Mzayad.Web.Resources;
using OrangeJetpack.Base.Web;
using System.Threading.Tasks;
using System.Web.Mvc;
using DetailsViewModel = Mzayad.Web.Models.Order.DetailsViewModel;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}/orders"), Authorize]
    public class OrdersController : ApplicationController
    {
        private readonly AuctionService _auctionService;
        private readonly OrderService _orderService;
        private readonly KnetService _knetService;
        private readonly AddressService _addressService;

        public OrdersController(IAppServices appServices) : base(appServices)
        {
            _auctionService = new AuctionService(DataContextFactory);
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

            return View(auction);
        }

        [Route("buy-now/{auctionId:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> BuyNow(int auctionId, FormCollection formCollection)
        {
            var auction = await _auctionService.GetAuction(auctionId, Language);
            if (auction == null)
            {
                return HttpNotFound();
            }

            if (!auction.BuyNowAvailable())
            {
                SetStatusMessage(Global.OutOfStockErrorMessage, StatusMessageType.Error);
                return RedirectToAction("BuyNow", new {auction.AuctionId});
            }

            var user = await AuthService.CurrentUser();
            user.Address = await _addressService.GetAddress(user.AddressId);

            var order = await _orderService.CreateOrderForBuyNow(auction, user);

            return RedirectToAction("Shipping", new { order.OrderId });
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

            SetStatusMessage("Congratulations {0} you've won! First things first, please enter your shipping address below.", user.FirstName);

            return await Shipping(orderId);
        }

        [Route("shipping/{orderId:int}")]
        public async Task<ActionResult> Shipping(int orderId)
        {
            var order = await _orderService.GetById(orderId);
            if (order == null)
            {
                return HttpNotFound();
            }

            var model = new ShippingAddressViewModel
            {
                Order = order,
                AddressViewModel = new AddressViewModel(order.Address).Hydrate(),
                ShippingAddress = order.Address,              
            };

            return View(model);
        }

        [Route("shipping/{orderId:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Shipping(int orderId, ShippingAddressViewModel model)
        {
            var order = await _orderService.GetById(orderId);
            if (order == null)
            {
                return HttpNotFound();
            }

            order.Address.Name = model.ShippingAddress.Name;
            order.Address.PhoneCountryCode = model.ShippingAddress.PhoneCountryCode;
            order.Address.PhoneLocalNumber = model.ShippingAddress.PhoneLocalNumber;
            order.Address.CountryCode = model.AddressViewModel.CountryCode;
            order.Address.CityArea = model.AddressViewModel.CityArea;
            order.Address.AddressLine1 = model.AddressViewModel.AddressLine1;
            order.Address.AddressLine2 = model.AddressViewModel.AddressLine2;
            order.Address.AddressLine3 = model.AddressViewModel.AddressLine3;
            order.Address.AddressLine4 = model.AddressViewModel.AddressLine4;
            order.Address.PostalCode =model.AddressViewModel.PostalCode;
            order.Address.StateProvince = model.AddressViewModel.StateProvince;

            // add local shipping charges
            order.Shipping = AppSettings.LocalShipping;
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
        public async Task<ActionResult> Success(int orderId, string paymentId = null)
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
                KnetTransaction = knetTransaction
            };

            return View(viewModel);
        }
    }
}