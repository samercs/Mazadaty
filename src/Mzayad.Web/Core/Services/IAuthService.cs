using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mzayad.Models;
using Mzayad.Web.Core.Identity;

namespace Mzayad.Web.Core.Services
{
    public interface IAuthService
    {
        bool IsAuthenticated();
        bool IsLocal();
        string CurrentUserId();
        string AnonymousId();
        string UserHostAddress();

        Task<ApplicationUser> SignIn(string userName, string password, bool rememberMe);
        Task<ApplicationUser> SignIn(ApplicationUser user);
        void SignOut();

        Task<ApplicationUser> CurrentUser();

        [Obsolete("Use Mzayad.Services.Identity.UserService instead.")]
        Task<ApplicationUser> GetUserById(string userId);
        [Obsolete("Use Mzayad.Services.Identity.UserService instead.")]
        Task<ApplicationUser> GetUserByName(string userName);
        [Obsolete("Use Mzayad.Services.Identity.UserService instead.")]
        Task<ApplicationUser> GetUserByEmail(string email);
        [Obsolete("Use Mzayad.Services.Identity.UserService instead.")]
        Task<IEnumerable<ApplicationUser>> GetUsers(string search = "", Role? roleName = null);

        Task<bool> UserExists(string userName);
        Task<IdentityResult> CreateUser(ApplicationUser user, string password, Func<ApplicationUser, Task> onSuccess = null);
        [Obsolete("Use Mzayad.Services.Identity.UserService instead.")]
        Task<IdentityResult> UpdateUser(ApplicationUser user);

        [Obsolete("Use Mzayad.Services.Identity.UserService instead.")]
        Task<IdentityResult> ChangePassword(IIdentity identity, string oldPassword, string newPassword);
        [Obsolete("Use Mzayad.Services.Identity.UserService instead.")]
        Task<IdentityResult> ChangePassword(string userId, string oldPassword, string newPassword);
        [Obsolete("Use Mzayad.Services.Identity.UserService instead.")]
        string HashPassword(string password);

        [Obsolete("Use Mzayad.Services.Identity.UserService instead.")]
        Task<IEnumerable<IdentityRole>> GetAllRoles();
        [Obsolete("Use Mzayad.Services.Identity.UserService instead.")]
        Task<IEnumerable<string>> GetRolesForUser(string userId);
        [Obsolete("Use Mzayad.Services.Identity.UserService instead.")]
        Task<bool> CurrentUserInRole(Role role);
        [Obsolete("Use Mzayad.Services.Identity.UserService instead.")]
        Task<bool> UserIsInRole(ApplicationUser user, Role role);
        [Obsolete("Use Mzayad.Services.Identity.UserService instead.")]
        Task AddUserToRole(string userId, string roleName);
        [Obsolete("Use Mzayad.Services.Identity.UserService instead.")]
        Task RemoveUserFromRole(string userId, string roleName);
    }
}
