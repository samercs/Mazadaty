﻿using System;
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
using Mzayad.Models;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Identity;

namespace Mzayad.Web.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpContextBase _httpContext;
        private readonly ApplicationUserManager _userManager;
        private readonly ApplicationRoleManager _roleManager;
        private readonly ApplicationSignInManager _signInManager;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly ICookieService _cookieService;
        
        public AuthService(HttpContextBase httpContext, ICookieService cookieService)
        {
            _httpContext = httpContext;
            _userManager = httpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            _roleManager = httpContext.GetOwinContext().Get<ApplicationRoleManager>();
            _signInManager = httpContext.GetOwinContext().Get<ApplicationSignInManager>();
            _authenticationManager = httpContext.GetOwinContext().Authentication;
            _cookieService = cookieService;
        }

        public bool IsAuthenticated()
        {
            return _httpContext.Request.IsAuthenticated;
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

        public async Task<bool> SignIn(string userName, string password, bool rememberMe)
        {
            var result = await _signInManager.PasswordSignInAsync(userName, password, rememberMe, false);
            if (result != SignInStatus.Success)
            {
                return false;
            }

            var user = await _userManager.FindByNameAsync(userName);
            SetLoginCookies(user);

            return true;
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

        public async Task<ApplicationUser> GetUserByLogin(string userName, string password)
        {
            return await _userManager.FindAsync(userName, password);
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser> GetUserByName(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsers(string search = "", string roleName = "")
        {
            var users = _userManager.Users;

            if (!string.IsNullOrWhiteSpace(search))
            {
                users = users.Where(i =>
                    i.FirstName.Contains(search) ||
                    i.LastName.Contains(search) ||
                    i.Email.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                var role = _roleManager.FindByName(roleName);

                if (role == null)
                    throw new ArgumentException("Please specify a valid role.");

                users = users.Where(i => i.Roles.Select(r => r.RoleId).Contains(role.Id));
            }

            return await users.ToListAsync();
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
            var roleManager = _httpContext.GetOwinContext().Get<ApplicationRoleManager>();
            var userRole = await roleManager.FindByNameAsync(role.ToString());
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