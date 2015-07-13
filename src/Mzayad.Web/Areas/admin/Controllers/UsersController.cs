using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Areas.admin.Models.Users;
using Mzayad.Web.Areas.Admin.Models.Users;
using Mzayad.Web.Core.ActionResults;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;
using OrangeJetpack.Base.Core.Formatting;

namespace Mzayad.Web.Areas.admin.Controllers
{
    [RouteArea("admin"), RoutePrefix("users"), RoleAuthorize(Role.Administrator)]
    public class UsersController : ApplicationController
    {
        private readonly SubscriptionService _subscriptionService;
        private readonly TokenService _tokenService;

        public UsersController(IAppServices appServices)
            : base(appServices)
        {
            _subscriptionService = new SubscriptionService(DataContextFactory);
            _tokenService = new TokenService(DataContextFactory);
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
            var roles = from Role r in Enum.GetValues(typeof(Role))
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

            var model = await new DetailsViewModel().Hydrate(user, AuthService, _subscriptionService, _tokenService);

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
                await model.Hydrate(user, AuthService, _subscriptionService, _tokenService);

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

        [Route("edit-subscription/{id}")]
        public async Task<ActionResult> EditSubscription(string id)
        {
            var user = await AuthService.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            var model = new EditSubscriptionViewModel().Hydrate(user);
            return View(model);
        }

        [Route("edit-subscription/{id}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> EditSubscription(EditSubscriptionViewModel model)
        {
            var user = await AuthService.GetUserById(model.User.Id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var modifiedSubscriptionUtc = model.CurrentSubscription.AddHours(-3); // AST -> UTC
            var currentUser = await AuthService.CurrentUser();
            var hostAddress = AuthService.UserHostAddress();

            await _subscriptionService.AddSubscriptionToUser(user, modifiedSubscriptionUtc, currentUser, hostAddress);

            SetStatusMessage("The user subscription has been updated successfully.");
            return RedirectToAction("Details", "Users", new { id = user.Id });
        }

        [HttpPost]
        public async Task<JsonResult> GetSubscriptionLog([DataSourceRequest] DataSourceRequest request, string id)
        {
            var logs = await _subscriptionService.GetSubscriptionLogsByUserId(id);
            return Json(logs.ToDataSourceResult(request));
        }
    }
}