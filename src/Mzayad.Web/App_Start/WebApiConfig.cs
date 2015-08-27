using Microsoft.Owin.Security.OAuth;
using Mzayad.Web.Core.Attributes;
using Newtonsoft.Json.Serialization;
using System.Web.Http;

namespace Mzayad.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

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

            // enforce HTTPS
            config.Filters.Add(new RequireHttpsAttribute());

            // set JSON serialization settings
            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
        }
    }
}
