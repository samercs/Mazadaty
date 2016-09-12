using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mzayad.Data;
using Mzayad.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Mzayad.Services.Identity
{
    public class UserService : ServiceBase
    {
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;

        public UserService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
            _userManager = new UserManager(dataContextFactory);
            _roleManager = new RoleManager(dataContextFactory);
        }

        public async Task<ApplicationUser> GetUserById(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser> GetUserByName(string username)
        {
            return await _userManager.FindByNameAsync(username);
        }

        public async Task<IReadOnlyCollection<ApplicationUser>> GetUsers(string search = "", string role = null)
        {
            var users = _userManager.Users;

            if (!string.IsNullOrWhiteSpace(search))
            {
                users = users.Where(i =>
                    i.FirstName.Contains(search) ||
                    i.LastName.Contains(search) ||
                    i.UserName.Contains(search) ||
                    i.Email.Contains(search));
            }

            if (!string.IsNullOrEmpty(role))
            {
                var identityRole = _roleManager.FindByName(role);
                if (identityRole == null)
                {
                    throw new ArgumentException("Cannot find role '" + role + "'.");
                }

                users = users.Where(i => i.Roles.Select(r => r.RoleId).Contains(identityRole.Id));
            }

            return await users.OrderBy(i => i.LastName).ThenBy(i => i.FirstName).ToListAsync();
        }

        public DateTime? GetUserSubscriptionUtc(string userId)
        {
            var user = _userManager.FindById(userId);
            return user?.SubscriptionUtc;
        }

        public async Task<IdentityResult> CreateUser(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task<IdentityResult> UpdateUser(ApplicationUser user)
        {
            var update = await _userManager.FindByIdAsync(user.Id);
            update.FirstName = user.FirstName;
            update.LastName = user.LastName;
            update.Email = user.Email;
            update.PhoneCountryCode = user.PhoneCountryCode;
            update.PhoneNumber = user.PhoneNumber;
            update.AvatarUrl = user.AvatarUrl;
            update.ProfileStatus = user.ProfileStatus;
            update.Gender = user.Gender;
            update.Birthdate = user.Birthdate;

            return await _userManager.UpdateAsync(update);
        }

        public async Task<IdentityResult> ChangePassword(string userId, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(userId, oldPassword, newPassword);
        }

        public string HashPassword(string password)
        {
            return _userManager.PasswordHasher.HashPassword(password);
        }

        public async Task<IReadOnlyCollection<IdentityRole>> GetAllRoles()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<IReadOnlyCollection<string>> GetRolesForUser(string userId)
        {
            return (await _userManager.GetRolesAsync(userId)).ToList();
        }

        public async Task<IEnumerable<ApplicationUser>> GetUsersInRole(string role)
        {
            var userRole = await _roleManager.FindByNameAsync(role);
            var userIds = userRole.Users.Select(i => i.UserId).ToList();
            var users = _userManager.Users.Where(i => userIds.Contains(i.Id));
            return await users.ToListAsync();
        }

        public async Task<bool> UserIsInRole(ApplicationUser user, string role)
        {
            return await _userManager.IsInRoleAsync(user.Id, role);
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
