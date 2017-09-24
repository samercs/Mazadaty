using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Mazadaty.Data;
using Mazadaty.Models;
using Mazadaty.Web.Core.Providers;
using Owin;
using System;
using System.Web.Http;

namespace Mazadaty.Web
{
    public partial class Startup
    {
        public static OAuthBearerAuthenticationOptions OAuthBearerOptions { get; private set; }

        public Startup()
        {
            OAuthBearerOptions = new OAuthBearerAuthenticationOptions();
        }

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

            var config = GlobalConfiguration.Configuration;
            config.Filters.Add(new HostAuthenticationFilter(OAuthBearerOptions.AuthenticationType));

            app.UseOAuthAuthorizationServer(new OAuthAuthorizationServerOptions
            {
#if DEBUG
                AllowInsecureHttp = true,
#endif

                TokenEndpointPath = new PathString("/api/token"),
                Provider = new ApplicationOAuthProvider("self"),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(365)
            });

            app.UseOAuthBearerAuthentication(OAuthBearerOptions);

        }
    }
}
