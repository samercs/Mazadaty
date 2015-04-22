using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Services;
using Mzayad.Web.Core.Services;

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

        public async Task<ActionResult> Index(int id)
        {
            var order = await _orderService.GetById(id);
            if (order == null)
            {
                return HttpNotFound("order not found");
            }
            return View(order);
        }
    }
}