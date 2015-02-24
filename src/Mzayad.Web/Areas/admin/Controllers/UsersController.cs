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
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;
using OrangeJetpack.Base.Core.Formatting;

namespace Mzayad.Web.Areas.admin.Controllers
{
    //[RoleAuthorize(Role.Administrator)]
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
                i.Id,
                i.FirstName,
                i.LastName,
                i.UserName,
                i.Email,
                i.CreatedUtc,
                PhoneNumber = i.PhoneCountryCode + i.PhoneNumber,
                Area = i.Address != null ? i.Address.CityArea : "",
                Block = i.Address != null ? i.Address.AddressLine1 : "",
                Street = i.Address != null ? i.Address.AddressLine2 : "",
                Building = i.Address != null ? i.Address.AddressLine3 : "",
                Jadda = i.Address != null ? i.Address.AddressLine4 : "",
                Country = i.Address != null ? i.Address.CountryCode : ""
            });

            return Excel(results, "users");
        }

        public async Task<ActionResult> Details(string id)
        {
            var user = await AuthService.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var model = await new DetailsViewModel().Hydrate(user, AuthService);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Details(string id, DetailsViewModel model, string[] selectedRoles)
        {
            var user = await AuthService.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            if (!ModelState.IsValid)
            {
                await model.Hydrate(user, AuthService);

                return View("Details", model);
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.UserName;
            user.Email = model.Email;
            
            await UpdateRoles(id, selectedRoles);
            await AuthService.UpdateUser(user);

            SetStatusMessage(string.Format("User {0} successfully updated.", user.UserName));

            return RedirectToAction("Details", new { id });
        }

        private async Task UpdateRoles(string userId, IEnumerable<string> selectedRoles)
        {
            await RemoveUserFromRoles(userId);

            if (selectedRoles != null)
            {
                await AddUserToRoles(userId, selectedRoles);
            }
        }

        private async Task RemoveUserFromRoles(string userId)
        {
            var roles = await AuthService.GetAllRoles();
            foreach (var role in roles)
            {
                await AuthService.RemoveUserFromRole(userId, role.Name);
            }
        }

        private async Task AddUserToRoles(string userId, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                await AuthService.AddUserToRole(userId, role);
            }
        }
	}
}