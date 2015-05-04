using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mzayad.Models.Enum;
using Mzayad.Services;
using Mzayad.Web.Areas.admin.Models.Orders;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;
using WebGrease.Css.Extensions;

namespace Mzayad.Web.Areas.admin.Controllers
{
    public class OrdersController : ApplicationController
    {
        private readonly OrderService _orderService;
        
        // GET: admin/Orders
        public OrdersController(IAppServices appServices) : base(appServices)
        {
            _orderService=new OrderService(DataContextFactory);
        }

        public async Task<ActionResult> Index(IndexViewModel model)
        {
            var status = model.Status ?? OrderStatus.Processing;
            var orders = await _orderService.GetOrders(status, model.Search);

            model.Status = model.Status ?? OrderStatus.Processing;
            if (orders != null)
            {
                orders.ForEach(i =>i.RecalculateTotal());
                
                model.Orders = orders;
            }

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
            var order = await _orderService.GetOrder(id);
            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }
    }
}