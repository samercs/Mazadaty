using Mazadaty.Services.Payment;
using Mazadaty.Web.Areas.Reports.Models.Transactions;
using Mazadaty.Web.Controllers;
using Mazadaty.Web.Core.Attributes;
using Mazadaty.Web.Core.Identity;
using Mazadaty.Web.Core.Services;
using Mazadaty.Web.Models;
using OrangeJetpack.Base.Web;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mazadaty.Services;

namespace Mazadaty.Web.Areas.Reports.Controllers
{
    [RoleAuthorize(Role.Administrator, Role.Accountant)]
    [RouteArea("reports"), RoutePrefix("transactions")]
    public class TransactionsController : ApplicationController
    {
        private readonly KnetService _knetService;
        private readonly OrderService _orderService;

        public TransactionsController(IAppServices appServices) : base(appServices)
        {
            _knetService = new KnetService(DataContextFactory);
            _orderService = new OrderService(DataContextFactory);
        }

        [Route("knet")]
        public async Task<ActionResult> Knet()
        {
            var model = new KnetViewModel
            {
                DateRange = new DateRangeModel
                {
                    StartDate = DateTime.Now.AddDays(-7),
                    EndDate = DateTime.Now
                }
            };

            model.KnetTransactions = await _knetService.GetTransactionsByDate(model.DateRange.StartDate, model.DateRange.EndDate);
            return View(model);
        }

        [Route("knet")]
        [HttpPost]
        public async Task<ActionResult> Knet(KnetViewModel model)
        {
            if (model.DateRange.StartDate > model.DateRange.EndDate)
            {
                SetStatusMessage("End date is greater than start date, please pick another dates and try again.", StatusMessageType.Warning);
                return View(model);
            }

            model.KnetTransactions = await _knetService.GetTransactionsByDate(model.DateRange.StartDate, model.DateRange.EndDate);
            return View(model);
        }

        [Route("buy-now")]
        public async Task<ActionResult> BuyNow()
        {
            var model = new BuyNowViewModel
            {
                DateRange = new DateRangeModel
                {
                    StartDate = DateTime.Now.AddDays(-7),
                    EndDate = DateTime.Now
                }
            };

            model.BuyNowTransactions = await _orderService.GetBuyNowTransactions(model.DateRange.StartDate, model.DateRange.EndDate);
            return View(model);
        }

        [Route("buy-now")]
        [HttpPost]
        public async Task<ActionResult> BuyNow(BuyNowViewModel model)
        {
            if (model.DateRange.StartDate > model.DateRange.EndDate)
            {
                SetStatusMessage("End date is greater than start date, please pick another dates and try again.", StatusMessageType.Warning);
                return View(model);
            }

            model.BuyNowTransactions = await _orderService.GetBuyNowTransactions(model.DateRange.StartDate, model.DateRange.EndDate);
            return View(model);
        }
    }
}
