using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Web.Core.Providers;
using Owin;
using System;

namespace Mzayad.Web
{
    public partial class Startup
    {      
        public void ConfigureAuth(IAppBuilder app)
        {
            app.CreatePerOwinContext(DataContext.Create);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/en/account/sign-in"),
                Provider = new CookieAuthenticationProvider
                {
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<Services.Identity.UserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });

            app.UseOAuthBearerTokens(new OAuthAuthorizationServerOptions
            {
#if DEBUG
                AllowInsecureHttp = true,
#endif

                TokenEndpointPath = new PathString("/api/token"),
                Provider = new ApplicationOAuthProvider("self"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(365)
            });
        }
    }
}