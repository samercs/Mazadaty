using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mzayad.Services;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.ActionResults;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Base.Core.Formatting;

namespace Mzayad.Web.Areas.Reports.Controllers
{
    public class SubscriptionsController : ApplicationController
    {

        private readonly SubscriptionLogService _subscriptionLogService;

        public SubscriptionsController(IAppServices appServices)
            : base(appServices)
        {
            _subscriptionLogService=new SubscriptionLogService(DataContextFactory);
        }
        
        // GET: Reports/Subscriptions
        public async Task<ActionResult> Index()
        {
            var model = await _subscriptionLogService.GetAll();
            return View(model);
        }

        [HttpPost]
        public async Task<JsonResult> GetSubscriptionLog([DataSourceRequest] DataSourceRequest request)
        {
            var result = await _subscriptionLogService.GetAll();
            return Json(result.ToDataSourceResult(request));
        }

        public async Task<ExcelResult> ToExcel()
        {
            var tmp = await _subscriptionLogService.GetAll();
            var results = tmp.Select(i => new
            {
                Id=i.SubscriptionLogId,
                DateTime= i.CreatedUtc,
                User=NameFormatter.GetFullName(i.User.FirstName,i.User.LastName),
                OrginalValue=i.OriginalSubscriptionUtc,
                NewValue=i.ModifiedSubscriptionUtc,
                Change="+" + i.DaysAdded + " Days",
                ChangedBy = NameFormatter.GetFullName(i.ModifiedByUser.FirstName, i.ModifiedByUser.LastName)
            });

            return Excel(results, "SubscriptionLog");
        }

        
    }
}