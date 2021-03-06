using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mazadaty.Core.Formatting;
using Mazadaty.Models;
using Mazadaty.Models.Enum;
using Mazadaty.Services;
using Mazadaty.Web.Areas.admin.Models.Orders;
using Mazadaty.Web.Controllers;
using Mazadaty.Web.Core.ActionResults;
using Mazadaty.Web.Core.Attributes;
using Mazadaty.Web.Core.Identity;
using Mazadaty.Web.Core.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mazadaty.Models.Enums;
using Mazadaty.Services.Messaging;
using Mazadaty.Web.Extensions;
using OrangeJetpack.Localization;
using WebGrease.Css.Extensions;


namespace Mazadaty.Web.Areas.admin.Controllers
{
    [RouteArea("admin"), RoutePrefix("auctions"), RoleAuthorize(Role.Administrator)]
    public class OrdersController : ApplicationController
    {
        private readonly OrderService _orderService;
        private readonly IAuthService _authService;
        private readonly IMessageService _messageService;
        private readonly EmailTemplateService _emailTemplateService;

        public OrdersController(IAppServices appServices) : base(appServices)
        {
            _orderService=new OrderService(DataContextFactory);
            _authService = appServices.AuthService;
            _messageService = appServices.MessageService;
            _emailTemplateService=new EmailTemplateService(DataContextFactory);
        }

        public async Task<ActionResult> Index(IndexViewModel model)
        {
            var status = model.Status ?? OrderStatus.Processing;
            var orders = await _orderService.GetOrders(status, model.Search);
            //orders.ForEach(i => i.RecalculateTotal());

            model.Status = model.Status ?? OrderStatus.Processing;
            model.Orders = orders.Select(OrderModel.Create);

            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetOrders([DataSourceRequest] DataSourceRequest request, OrderStatus orderStatus, string search = "")
        {
            var orders = await _orderService.GetOrders(orderStatus, search);
            var results = orders;
            return Json(results.ToDataSourceResult(request));
        }

        public async Task<ActionResult> Details(int id)
        {
            var order = await _orderService.GetById(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }


        public async Task<ActionResult> Ship(int id)
        {
            var order = await _orderService.GetById(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Ship(int id, bool sendNotification = false)
        {
            var order = await _orderService.GetById(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            await _orderService.SaveOrderAsDelivered(order, _authService.CurrentUserId());

            if (sendNotification)
            {
                await SendOrderShipmentNotification(order);
            }

            SetStatusMessage("Order status updated to SHIPPED.");

            return RedirectToAction("Details", new { id });
        }

        private async Task SendOrderShipmentNotification(Order order)
        {
            var emailTemplate = await _emailTemplateService.GetByTemplateType(EmailTemplateType.OrderShipped);
            var email = new Email
            {
                ToAddress = order.User.Email,
                Subject = emailTemplate.Localize("en", i => i.Subject).Subject,
                Message = string.Format(emailTemplate.Localize("en", i => i.Message).Message, order.User.FirstName)
            };

            await _messageService.Send(email.WithTemplate());
        }

        public async Task<ExcelResult> OrdersExcel( OrderStatus status,string search = "")
        {
            var orders = await _orderService.GetOrders(status,search);
            orders.ForEach(i=>i.RecalculateTotal());
            var results = orders.Select(i => new
            {
                i.OrderId,
                i.Address.Name,
                i.Subtotal,
                i.Shipping,
                i.Total,
                Items=i.Items.Count,
                i.SubmittedUtc,
                i.ShippedUtc,
                PhoneNumber = PhoneNumberFormatter.Format(i.Address.PhoneCountryCode,i.Address.PhoneLocalNumber),
                Area = i.Address != null ? i.Address.CityArea : "",
                Block = i.Address != null ? i.Address.AddressLine1 : "",
                Street = i.Address != null ? i.Address.AddressLine2 : "",
                Building = i.Address != null ? i.Address.AddressLine3 : "",
                Jadda = i.Address != null ? i.Address.AddressLine4 : "",
                Country = i.Address != null ? i.Address.CountryCode : ""
            });

            return Excel(results, "orders");
        }

    }
}
