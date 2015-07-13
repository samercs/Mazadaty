using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mzayad.Services;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.ActionResults;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Base.Core.Formatting;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Web.Areas.Reports.Models.Subscriptions;
using Mzayad.Web.Areas.Reports.Models.Tokens;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;

namespace Mzayad.Web.Areas.Reports.Controllers
{
    [RoleAuthorize(Role.Administrator)]
    [RouteArea("reports"), RoutePrefix("tokens")]
    public class TokensController : ApplicationController
    {
        private readonly TokenService _tokenService;

        public TokensController(IAppServices appServices)
            : base(appServices)
        {
            _tokenService = new TokenService(DataContextFactory);
        }

        [Route("logs")]
        public async Task<ActionResult> Logs()
        {
            var subscriptionLogs = await _tokenService.GetTokenLogs();
            var viewModel = subscriptionLogs.Select(TokenLogViewModel.Create);
            
            return View(viewModel);
        }

        [HttpPost]
        [Route("logs/json")]
        public async Task<JsonResult> LogsAsJson([DataSourceRequest] DataSourceRequest request)
        {
            var subscriptionLogs = await _tokenService.GetTokenLogs();
            var results = subscriptionLogs.Select(TokenLogViewModel.Create);
            
            return Json(results.ToDataSourceResult(request));
        }

        [Route("logs/excel")]
        public async Task<ExcelResult> LogsAsExcel()
        {
            var subscriptionLogs = await _tokenService.GetTokenLogs();
            var results = subscriptionLogs.Select(TokenLogViewModel.Create);

            return Excel(results, "Token Logs");
        }


    }
}