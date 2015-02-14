using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Mzayad.Web.Core.Configuration;

namespace Mzayad.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutofacConfig.RegisterAll();
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            var cultureInfo = GetCultureFromRoute() ?? GetCultureFromCookie() ?? GetCultureFromThread();
            
            cultureInfo.DateTimeFormat.Calendar = new GregorianCalendar();
            cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";

            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }

        private CultureInfo GetCultureFromRoute()
        {
            Trace.TraceInformation("Trying GetCultureFromRoute()...");
            
            var handler = Context.Handler as MvcHandler;
            var routeData = handler == null ? null : handler.RequestContext.RouteData;
            var languageRoute = routeData == null ? null : routeData.Values["language"];        
            return languageRoute == null ? null : CultureInfo.CreateSpecificCulture(languageRoute.ToString());
        }

        private static CultureInfo GetCultureFromCookie()
        {
            Trace.TraceInformation("Trying GetCultureFromCookie()...");
            
            var languageCookie = HttpContext.Current.Request.Cookies.Get(CookieKeys.LanguageCode);
            return languageCookie == null ? null : CultureInfo.CreateSpecificCulture(languageCookie.Value);
        }

        private static CultureInfo GetCultureFromThread()
        {
            Trace.TraceInformation("Trying GetCultureFromThread()... [{0}]", Thread.CurrentThread.CurrentCulture.Name);
            
            if (Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar", StringComparison.OrdinalIgnoreCase))
            {
                return CultureInfo.CreateSpecificCulture("ar");
            }

            return CultureInfo.CreateSpecificCulture("en");
        }
    }
}
