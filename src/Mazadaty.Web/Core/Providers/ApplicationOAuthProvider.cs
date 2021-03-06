using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Mazadaty.Data;
using Mazadaty.Models;
using Mazadaty.Services.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mazadaty.Services;

namespace Mazadaty.Web.Core.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException(nameof(publicClientId));
            }

            _publicClientId = publicClientId;
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var dataContextFactory = new DataContextFactory();
            var userManager = new UserManager(dataContextFactory);
            var addressService = new AddressService(dataContextFactory);

            var userName = context.UserName;
            if (userName.Contains("@"))
            {
                var emailUser = await userManager.FindByEmailAsync(userName);
                if (emailUser != null)
                {
                    userName = emailUser.UserName;
                }
            }

            var user = await userManager.FindAsync(userName, context.Password);
            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }

            var address = await addressService.GetAddress(user.AddressId);
            if (address != null)
            {
                user.Address = address;
            }

            var oAuthIdentity = await user.GenerateUserIdentityAsync(userManager, OAuthDefaults.AuthenticationType);
            var cookiesIdentity = await user.GenerateUserIdentityAsync(userManager, CookieAuthenticationDefaults.AuthenticationType);

            var properties = CreateProperties(user);
            var ticket = new AuthenticationTicket(oAuthIdentity, properties);

            context.Validated(ticket);
            context.Request.Context.Authentication.SignIn(cookiesIdentity);
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (var property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                var expectedRootUri = new Uri(context.Request.Uri, "/");
                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }

        public static AuthenticationProperties CreateProperties(ApplicationUser user)
        {
            IDictionary<string, string> data = new Dictionary<string, string>
            {
                { "userId", user.Id },
                { "userName", user.UserName },
                { "fullName", user.FullName },
                { "email", user.Email },
                { "countryCode", user.Address?.CountryCode ?? "KW" },
                { "avatarUrl", user.AvatarUrl }
            };

            return new AuthenticationProperties(data);
        }
    }
}
