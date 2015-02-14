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

        Task<ApplicationUser> SignIn(string userName, string password, bool rememberMe);
        Task<ApplicationUser> SignIn(ApplicationUser user);
        void SignOut();

        Task<ApplicationUser> CurrentUser();
        Task<ApplicationUser> GetUserById(string userId);
        Task<ApplicationUser> GetUserByName(string userName);
        Task<ApplicationUser> GetUserByEmail(string email);
        Task<IEnumerable<ApplicationUser>> GetUsers(string search = "", string roleName = "");

        Task<bool> UserExists(string userName);
        Task<IdentityResult> CreateUser(ApplicationUser user, string password, Func<ApplicationUser, Task> onSuccess = null);
        Task<IdentityResult> UpdateUser(ApplicationUser user);
        
        Task<IdentityResult> ChangePassword(IIdentity identity, string oldPassword, string newPassword);
        string HashPassword(string password);

        Task<IEnumerable<IdentityRole>> GetAllRoles();
        Task<IEnumerable<string>> GetRolesForUser(string userId);
        Task<bool> CurrentUserInRole(Role role);
        Task<bool> UserIsInRole(ApplicationUser user, Role role);
        Task AddUserToRole(string userId, string roleName);
        Task RemoveUserFromRole(string userId, string roleName);
    }
}
