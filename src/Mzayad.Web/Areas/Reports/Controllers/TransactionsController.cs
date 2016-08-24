using Mzayad.Services.Payment;
using Mzayad.Web.Areas.Reports.Models.Transactions;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models;
using OrangeJetpack.Base.Web;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Areas.Reports.Controllers
{
    [RoleAuthorize(Role.Administrator)]
    [RouteArea("reports"), RoutePrefix("transactions")]
    public class TransactionsController : ApplicationController
    {
        private readonly KnetService _knetService;

        public TransactionsController(IAppServices appServices) : base(appServices)
        {
            _knetService = new KnetService(DataContextFactory);
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
    }
}