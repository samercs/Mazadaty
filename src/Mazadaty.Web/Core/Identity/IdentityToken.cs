using Microsoft.Owin.Security;
using Mazadaty.Models;
using System;
using System.Security.Claims;

namespace Mazadaty.Web.Core.Identity
{
    public static class IdentityToken
    {
        public static string GetToken(ApplicationUser user)
        {
            if (user == null)
            {
                return string.Empty;
            }

            var identity = new ClaimsIdentity(Startup.OAuthBearerOptions.AuthenticationType);

            identity.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

            var ticket = new AuthenticationTicket(identity, new AuthenticationProperties());

            var currentUtc = DateTime.UtcNow;
            ticket.Properties.IssuedUtc = currentUtc;
            ticket.Properties.ExpiresUtc = currentUtc.Add(TimeSpan.FromMinutes(30));

            return Startup.OAuthBearerOptions.AccessTokenFormat.Protect(ticket);
        }
    }
}
