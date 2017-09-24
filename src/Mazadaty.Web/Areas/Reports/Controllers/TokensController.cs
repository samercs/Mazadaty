using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mazadaty.Services;
using Mazadaty.Web.Areas.Reports.Models.Tokens;
using Mazadaty.Web.Controllers;
using Mazadaty.Web.Core.ActionResults;
using Mazadaty.Web.Core.Attributes;
using Mazadaty.Web.Core.Identity;
using Mazadaty.Web.Core.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mazadaty.Web.Areas.Reports.Controllers
{
    [RoleAuthorize(Role.Administrator, Role.Accountant)]
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
