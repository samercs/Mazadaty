using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Mazadaty.Data;
using Mazadaty.Models;
using Mazadaty.Services.Identity;

namespace Mazadaty.Web.Core.Identity
{
    public class SignInManager : SignInManager<ApplicationUser, string>
    {
        public SignInManager(UserManager<ApplicationUser, string> userManager, IAuthenticationManager authenticationManager) : 
            base(userManager, authenticationManager) { }

        public static SignInManager Create(IdentityFactoryOptions<SignInManager> options, IOwinContext context)
        {
            return new SignInManager(new UserManager(new DataContextFactory()), context.Authentication);
        }
    }
}
