using Mzayad.Core.Formatting;
using Mzayad.Services;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.Order;
using Mzayad.Web.Models.Shared;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Controllers
{
    [RoutePrefix("orders")]
    public class OrdersController : ApplicationController
    {
        private readonly OrderService _orderService;

        public OrdersController(IAppServices appServices) : base(appServices)
        {
            _orderService = new OrderService(DataContextFactory);
        }

        [Route("{orderId:int}/shipping")]
        public async Task<ActionResult> Shipping(int orderId)
        {
            var order = await _orderService.GetById(orderId);
            if (order == null)
            {
                return HttpNotFound("order not found");
            }

            var phoneNumber = order.Address.PhoneNumber.Split(' ');
            var model = new ShippingAddressViewModel()
            {
                Order = order,
                AddressViewModel = new AddressViewModel(order.Address).Hydrate(),
                ShippingAddress = order.Address,
                PhoneNumberCountryCode = phoneNumber[0],
                PhoneNumberNumber = phoneNumber[1]               
            };

            return View(model);
        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<ActionResult> Shipping(int id,ShippingAddressViewModel model)
        {
            var order = await _orderService.GetById(id);
            if (order == null)
            {
                return HttpNotFound("order not found");
            }

            order.AllowPhoneSms = model.Order.AllowPhoneSms;

            order.Address.FirstName = model.ShippingAddress.FirstName;
            order.Address.LastName = model.ShippingAddress.LastName;
            order.Address.PhoneNumber = PhoneNumberFormatter.Format(model.PhoneNumberCountryCode,
                model.PhoneNumberNumber);
            order.Address.CountryCode = model.AddressViewModel.CountryCode;
            order.Address.CityArea = model.AddressViewModel.CityArea;
            order.Address.AddressLine1 = model.AddressViewModel.AddressLine1;
            order.Address.AddressLine2 = model.AddressViewModel.AddressLine2;
            order.Address.AddressLine3 = model.AddressViewModel.AddressLine3;
            order.Address.AddressLine4 = model.AddressViewModel.AddressLine4;
            order.Address.PostalCode =model.AddressViewModel.PostalCode;
            order.Address.StateProvince = model.AddressViewModel.StateProvince;

            await _orderService.Update(order);

            return RedirectToAction("Summary", new {id = id});
        }

        public async Task<ActionResult> Summary(int id)
        {
            var order = await _orderService.GetById(id);
            if (order == null)
            {
                return HttpNotFound("Order not found");
            }

            var model = new OrderSummaryViewModel()
            {
                Order = order,
                Language = Language
            };

            return View(model);

            

        }

        [HttpPost,ValidateAntiForgeryToken]
        public async Task<ActionResult> Summary(int id,OrderSummaryViewModel model)
        {
            var order = await _orderService.GetById(id);
            if (order == null)
            {
                return HttpNotFound("Order not found");
            }

            order.PaymentMethod = model.Order.PaymentMethod;
            await _orderService.Update(order);


            return RedirectToAction("Submit", new {id=id});



        }

        public ActionResult Submit(int id)
        {
            return View();
        }
    }
}