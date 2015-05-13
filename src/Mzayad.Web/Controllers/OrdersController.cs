using Mzayad.Services;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.Order;
using Mzayad.Web.Models.Shared;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Services.Payment;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("{language}/orders")]
    public class OrdersController : ApplicationController
    {
        private readonly OrderService _orderService;

        public OrdersController(IAppServices appServices) : base(appServices)
        {
            _orderService = new OrderService(DataContextFactory);
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

            var user = await AuthService.CurrentUser();

            SetStatusMessage("Congratulations {0} you've won! First things first, please enter your shipping address below.", user.FirstName);

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

        [Route("summary/{orderId:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Summary(int orderId, OrderSummaryViewModel model)
        {
            var order = await _orderService.GetById(orderId);
            if (order == null)
            {
                return HttpNotFound();
            }

            //order.PaymentMethod = model.Order.PaymentMethod;
            //await _orderService.Update(order);

            var knetService = new KnetService(DataContextFactory);

            // TODO: KNET

            return RedirectToAction("Submit", new { orderId });
        }

        [Route("submit/{orderId:int}")]
        public ActionResult Submit(int orderId)
        {
            
            
            return Content("submit");
        }
    }
}