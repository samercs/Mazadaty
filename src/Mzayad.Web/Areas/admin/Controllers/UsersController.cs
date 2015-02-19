using System;
using System.Collections.Generic;
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
using OrangeJetpack.Base.Core.Formatting;

namespace Mzayad.Web.Areas.admin.Controllers
{
    public class UsersController : ApplicationController
    {
        public UsersController(IControllerServices controllerServices) : base(controllerServices)
        {
        }

        public async Task<ActionResult> Index(string search = "", Role? role = null)
        {
            var viewModel = new IndexViewModel
            {
                Search = search,
                Users = await AuthService.GetUsers(search, role),
                Role = role,
                RoleList = GetRoleList()
            };

            return View(viewModel);
        }

        private static SelectList GetRoleList()
        {
            var roles = from Role r in Enum.GetValues(typeof (Role))
                select new
                {
                    Id = r, 
                    Name = EnumFormatter.Description(r)
                };
            
            return new SelectList(roles, "Id", "Name");
        }

        [HttpPost]
        public async Task<JsonResult> GetUsers([DataSourceRequest] DataSourceRequest request, string search = null, Role? role = null)
        {
            var results = await AuthService.GetUsers(search, role);
            return Json(results.ToDataSourceResult(request));
        }

        public async Task<ExcelResult> UsersExcel(string search = "", Role? role = null)
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