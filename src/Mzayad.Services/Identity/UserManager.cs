using System;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Mzayad.Data;
using Mzayad.Models;

namespace Mzayad.Services.Identity
{
    public class UserManager : UserManager<ApplicationUser>
    {
        public UserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
            InitUserManager();
        }
        
        public UserManager(IDataContextFactory dataContextFactory)
            : base(new UserStore<ApplicationUser>((DbContext)dataContextFactory.GetContext()))
        {
            InitUserManager();
        }

        private void InitUserManager()
        {
            UserValidator = new UserValidator<ApplicationUser>(this)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };

            PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = false,
                RequireDigit = false,
                RequireLowercase = false,
                RequireUppercase = false,
            };

            UserLockoutEnabledByDefault = true;
            DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(10);
            MaxFailedAccessAttemptsBeforeLockout = 10;
        }
    }
}
