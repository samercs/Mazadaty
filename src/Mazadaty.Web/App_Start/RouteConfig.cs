using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Mazadaty.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.LowercaseUrls = true;
            routes.MapMvcAttributeRoutes();

            AreaRegistration.RegisterAllAreas();

            routes.MapRoute(
                "UserProfile",
                "{language}/profiles/{userName}",
                new { controller = "User", action = "UserProfile" }
            );

            routes.MapRoute(
                "Localization",
                "{language}/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                new { language = @"^en|ar?" }
            );

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            
        }
    }
}
