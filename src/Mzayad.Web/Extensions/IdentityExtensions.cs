using System.Security.Principal;
using System.Web;
using Microsoft.AspNet.Identity;
using Mzayad.Web.Core.Configuration;
using OrangeJetpack.Base.Web.HttpServices;


namespace Mzayad.Web.Extensions
{
    public static class IdentityExtensions
    {
        /// <summary>
        /// Gets an authenticated user's first name or user name.
        /// </summary>
        public static string GetDisplayName(this IIdentity identity)
        {
            var cookieService = new CookieService(new HttpContextWrapper(HttpContext.Current));
            var displayName = cookieService.Get(CookieKeys.DisplayName);
            if (string.IsNullOrWhiteSpace(displayName))
            {
                displayName = identity.GetUserName();
            }

            return displayName;
        }
    }
}