using Microsoft.Owin.Security.OAuth;
using Mzayad.Web.Core.Attributes;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Mzayad.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            //config.EnableCors(); // TODO this doesn't add CORs headers, added to web.config instead

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "ActionApi",
                routeTemplate: "api/{controller}/action/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            

            // Enforce HTTPS
            config.Filters.Add(new RequireHttpsAttribute());
        }
    }
}
