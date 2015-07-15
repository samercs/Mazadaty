using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Services.Identity;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Identity;

namespace Mzayad.Web.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpContextBase _httpContext;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly SignInManager _signInManager;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly ICookieService _cookieService;
        
        public AuthService(HttpContextBase httpContext, ICookieService cookieService, IDataContextFactory dataContextFactory)
        {
            _httpContext = httpContext;
            _userManager = new UserManager(dataContextFactory);
            _roleManager = new RoleManager(dataContextFactory);
            _signInManager = new SignInManager(_userManager, _httpContext.GetOwinContext().Authentication);
            _authenticationManager = httpContext.GetOwinContext().Authentication;
            _cookieService = cookieService;
        }

        public bool IsAuthenticated()
        {
            return _httpContext.Request.IsAuthenticated;
        }

        public bool IsLocal()
        {
            return _httpContext.Request.IsLocal;
        }

        public string CurrentUserId()
        {
            if (!IsAuthenticated())
            {
                return null;
            }
           
            return _httpContext.User.Identity.GetUserId();
        }

        public string AnonymousId()
        {
            return _httpContext.Request.AnonymousID;
        }

        public string UserHostAddress()
        {
            return _httpContext.Request.UserHostAddress;
        }

        public async Task<ApplicationUser> SignIn(string username, string password, bool rememberMe)
        {
            ApplicationUser user;

            if (username.Contains("@"))
            {
                user = await _userManager.FindByEmailAsync(username);
            }
            else
            {
                user = await _userManager.FindByNameAsync(username);
            }

            if (user == null)
            {
                return null;
            }

            var result = await _signInManager.PasswordSignInAsync(user.UserName, password, rememberMe, false);
            if (result != SignInStatus.Success)
            {
                return null;
            }
        
            SetLoginCookies(user);

            return user;
        }

        public async Task<ApplicationUser> SignIn(ApplicationUser user)
        {
            await _signInManager.SignInAsync(user, false, false);
            SetLoginCookies(user);
            return user;
        }

        public void SignOut()
        {
            _authenticationManager.SignOut();
        }

        public async Task<ApplicationUser> CurrentUser()
        {
            if (!IsAuthenticated())
            {
                return null;
            }

            return await _userManager.FindByIdAsync(CurrentUserId());
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser> GetUserByName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsers(string search = "", Role? role = null)
        {
            var users = _userManager.Users.Include(i => i.Address);

            if (!string.IsNullOrWhiteSpace(search))
            {
                users = users.Where(i =>
                    i.FirstName.Contains(search) ||
                    i.LastName.Contains(search) ||
                    i.UserName.Contains(search) ||
                    i.Email.Contains(search));
            }

            if (role != null)
            {
                var identityRole = _roleManager.FindByName(role.ToString());
                if (identityRole == null)
                {
                    throw new ArgumentException("Please specify a valid role.");
                }

                users = users.Where(i => i.Roles.Select(r => r.RoleId).Contains(identityRole.Id));
            }

            return await users
                .OrderBy(i => i.LastName)
                .ThenBy(i => i.FirstName)
                .ThenBy(i => i.UserName)
                .ToListAsync();
        }

        public async Task<bool> UserExists(string userName)
        {
            return (await GetUserByName(userName)) != null;
        }

        public async Task<IdentityResult> CreateUser(ApplicationUser user, string password, Func<ApplicationUser, Task> onSuccess = null)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return result;
            }

            await _signInManager.SignInAsync(user, false, false);
            SetLoginCookies(user);

            if (onSuccess != null)
            {
                await onSuccess(user);
            }

            return result;
        }

        private void SetLoginCookies(ApplicationUser user)
        {
            _cookieService.Add(CookieKeys.DisplayName, user.FirstName, DateTime.MaxValue);
            _cookieService.Add(CookieKeys.LastSignInEmail, user.Email, DateTime.MaxValue);
        }

        public async Task<IdentityResult> UpdateUser(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangePassword(IIdentity identity, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(identity.GetUserId(), oldPassword, newPassword);
        }

        public async Task<IdentityResult> ChangePassword(string userId, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(userId, oldPassword, newPassword);
        }

        public string HashPassword(string password)
        {
            return _userManager.PasswordHasher.HashPassword(password);
        }

        public async Task<IEnumerable<IdentityRole>> GetAllRoles()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<IEnumerable<string>> GetRolesForUser(string userId)
        {
            return await _userManager.GetRolesAsync(userId);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersInRole(Role role)
        {
            var userRole = await _roleManager.FindByNameAsync(role.ToString());
            var userIds = userRole.Users.Select(i => i.UserId).ToList();
            var users = _userManager.Users.Where(i => userIds.Contains(i.Id));
            return await users.ToListAsync();
        }

        public async Task<bool> CurrentUserInRole(Role role)
        {
            if (!_httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            var user = await CurrentUser();
            return await UserIsInRole(user, role);
        }

        public async Task<bool> UserIsInRole(ApplicationUser user, Role role)
        {
            return await _userManager.IsInRoleAsync(user.Id, role.ToString());
        }

        public async Task AddUserToRole(string userId, string roleName)
        {
            await _userManager.AddToRoleAsync(userId, roleName);
        }

        public async Task RemoveUserFromRole(string userId, string roleName)
        {
            await _userManager.RemoveFromRoleAsync(userId, roleName);
        }

       
    }
}