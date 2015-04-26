using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Core.Formatting;
using Mzayad.Services;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.Order;
using Mzayad.Web.Models.Shared;

namespace Mzayad.Web.Controllers
{
    
    public class OrderController : ApplicationController
    {
        private readonly OrderService _orderService;

        // GET: Order
        public OrderController(IAppServices appServices) : base(appServices)
        {

            _orderService=new OrderService(DataContextFactory);
        }

        public  ActionResult Index(int id)
        {
            return RedirectToAction("Shipping", new {id = id});
        }

        public async Task<ActionResult> Shipping(int id)
        {
            var order = await _orderService.GetById(id);
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
    }
}