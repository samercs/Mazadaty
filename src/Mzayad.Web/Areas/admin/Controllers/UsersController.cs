using System;
using System.Linq;
using System.Threading.Tasks;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mzayad.Web.Areas.admin.Models.Users;
using Mzayad.Web.Core.ActionResults;
using Mzayad.Web.Core.Identity;

namespace Mzayad.Web.Areas.admin.Controllers
{
    public class UsersController : ApplicationController
    {
        public UsersController(IControllerServices controllerServices) : base(controllerServices)
        {
        }

        public async Task<ActionResult> Index(string search = "", string role = "")
        {
            var roles = from Role r in Enum.GetValues(typeof(Role))
                        orderby r.ToString()
                        select new { Id = r, Name = r.ToString() };

            var viewModel = new IndexViewModel
            {
                Search = search,
                Users = await AuthService.GetUsers(search, role),
                Role = role,
                Roles = new SelectList(roles, "Id", "Name")
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<JsonResult> GetUsers([DataSourceRequest] DataSourceRequest request, string search = "", string role = "")
        {
            var results = await AuthService.GetUsers(search, role);
            return Json(results.ToDataSourceResult(request));
        }

        public async Task<ExcelResult> UsersExcel(string search = "", string role = "")
        {
            var users = await AuthService.GetUsers(search, role);
            var results = users.Select(i => new
            {
                i.FirstName,
                i.LastName,
                i.UserName,
                i.Email,
                i.CreatedUtc,
                PhoneNumber = i.PhoneCountryCode + i.PhoneNumber
            });

            return Excel(results, "users");
        }
	}
}