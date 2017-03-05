using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mzayad.Services;
using Mzayad.Services.Identity;
using Mzayad.Web.Areas.admin.Models.Users;
using Mzayad.Web.Areas.Admin.Models.Users;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.ActionResults;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Base.Core.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using OrangeJetpack.Base.Web;

namespace Mzayad.Web.Areas.admin.Controllers
{
    [RouteArea("admin"), RoutePrefix("users"), RoleAuthorize(Role.Administrator)]
    public class UsersController : ApplicationController
    {
        private readonly UserService _userService;
        private readonly SubscriptionService _subscriptionService;
        private readonly TokenService _tokenService;
        private readonly TrophyService _trophyService;
        private readonly AuctionService _auctionService;
        private readonly BidService _bidService;

        public UsersController(IAppServices appServices) : base(appServices)
        {
            _userService = new UserService(DataContextFactory);
            _subscriptionService = new SubscriptionService(DataContextFactory);
            _tokenService = new TokenService(DataContextFactory);
            _trophyService = new TrophyService(DataContextFactory);
            _auctionService = new AuctionService(DataContextFactory, appServices.QueueService);
            _bidService = new BidService(DataContextFactory, appServices.QueueService);
        }

        public async Task<ActionResult> Index(string search = "", Role? role = null)
        {
            var viewModel = new IndexViewModel
            {
                Search = search,
                Users = await _userService.GetUsers(search, role.ToString()),
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
            var results = await _userService.GetUsers(search, role.ToString());
            return Json(results.ToDataSourceResult(request));
        }

        public async Task<ExcelResult> UsersExcel(string search = "", Role? role = null)
        {
            var users = await _userService.GetUsers(search, role.ToString());
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
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var model = await new DetailsViewModel
            {
                SubscriptionLogs = await _subscriptionService.GetSubscriptionLogsByUserId(id),
                TokenLogs = await _tokenService.GetTokenLogsByUserId(id),
                Trophies = await _trophyService.GetUserTrophies(id, Language),
                Auctions = await _auctionService.GetAuctionsWon(user.Id, Language),
                Bids = await _bidService.GetRecentBidHistoryForUser(user.Id, Language),
                IsLocked = await AuthService.IsLocked(id)
            }.Hydrate(user, _userService);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Details(string id, DetailsViewModel model, string[] selectedRoles)
        {
            var user = await _userService.GetUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            if (!ModelState.IsValid)
            {
                model.SubscriptionLogs = await _subscriptionService.GetSubscriptionLogsByUserId(id);
                model.TokenLogs = await _tokenService.GetTokenLogsByUserId(id);
                model.Trophies = await _trophyService.GetUserTrophies(id, Language);
                model.Auctions = await _auctionService.GetAuctionsWon(user.Id, Language);
                model.Bids = await _bidService.GetRecentBidHistoryForUser(user.Id, Language);
                await model.Hydrate(user, _userService);

                return View("Details", model);
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.UserName;
            user.Email = model.Email;

            await UpdateRoles(id, selectedRoles);
            await _userService.UpdateUser(user);

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
            var roles = await _userService.GetAllRoles();
            foreach (var role in roles)
            {
                await _userService.RemoveUserFromRole(userId, role.Name);
            }
        }

        private async Task AddUserToRoles(string userId, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                await _userService.AddUserToRole(userId, role);
            }
        }

        [Route("edit-subscription/{id}")]
        public async Task<ActionResult> EditSubscription(string id)
        {
            var user = await _userService.GetUserById(id);
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
            var user = await _userService.GetUserById(model.User.Id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var modifiedSubscriptionUtc = model.CurrentSubscription.AddHours(-3); // AST -> UTC
            var currentUserId = AuthService.CurrentUserId();
            var hostAddress = AuthService.UserHostAddress();

            await _subscriptionService.AddSubscriptionToUser(user, modifiedSubscriptionUtc, currentUserId, hostAddress);

            SetStatusMessage("The user subscription has been updated successfully.");
            return RedirectToAction("Details", "Users", new { id = user.Id });
        }

        [HttpPost]
        public async Task<JsonResult> GetSubscriptionLog([DataSourceRequest] DataSourceRequest request, string id)
        {
            var logs = await _subscriptionService.GetSubscriptionLogsByUserId(id);
            return Json(logs.ToDataSourceResult(request));
        }

        [Route("{userId}/disable")]
        public async Task<ActionResult> Disable(string userId)
        {
            var user = await _userService.GetUserById(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            return DeleteConfirmation("Disable User", "Are you sure you want to permanently disable this user?");
        }

        [Route("{userId}/disable")]
        [HttpPost]
        public async Task<ActionResult> DisableUser(string userId)
        {
            await AuthService.Lock(userId);
            return RedirectToAction("Details", new { id = userId });
        }
    }
}