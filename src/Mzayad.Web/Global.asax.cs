using Microsoft.AspNet.Identity;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.ModelBinder;
using Newtonsoft.Json;
using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Mzayad.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutofacConfig.RegisterAll();

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, Data.Migrations.Configuration>());

            ModelBinders.Binders.Add(typeof(DateTime), new DateTimeBinder());
            ModelBinders.Binders.Add(typeof(DateTime?), new NullableDateTimeBinder());

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }

        protected void Application_BeginRequest()
        {
            if (Request.Headers.AllKeys.Contains("Origin") && Request.HttpMethod == "OPTIONS")
            {
                Response.Flush();
            }
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            var cultureInfo = GetCultureFromRoute() ?? GetCultureFromCookie() ?? GetCultureFromThread();

            cultureInfo.DateTimeFormat.Calendar = new GregorianCalendar();
            cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";

            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            if (HttpContext.Current.User.Identity.IsAuthenticated)
            {
                var sessionLog = new SessionLogService(new DataContextFactory());
                try
                {
                    sessionLog.Insert(new SessionLog()
                    {
                        Browser = Request.UserAgent,
                        IP = Request.UserHostAddress,
                        UserId = HttpContext.Current.User.Identity.GetUserId()
                    });

                }
                catch
                {
                    //ToDo handel session insertion falier.
                }

            }
        }

        private CultureInfo GetCultureFromRoute()
        {
            var handler = Context.Handler as MvcHandler;
            var routeData = handler == null ? null : handler.RequestContext.RouteData;
            var languageRoute = routeData == null ? null : routeData.Values["language"];
            return languageRoute == null ? null : CultureInfo.CreateSpecificCulture(languageRoute.ToString());
        }

        private static CultureInfo GetCultureFromCookie()
        {
            var languageCookie = HttpContext.Current.Request.Cookies.Get(CookieKeys.LanguageCode);
            return languageCookie == null ? null : CultureInfo.CreateSpecificCulture(languageCookie.Value);
        }

        private static CultureInfo GetCultureFromThread()
        {
            if (Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar", StringComparison.OrdinalIgnoreCase))
            {
                return CultureInfo.CreateSpecificCulture("ar");
            }

            return CultureInfo.CreateSpecificCulture("en");
        }

        public override string GetVaryByCustomString(HttpContext context, string arg)
        {
            if (arg == "User")
            {
                return "User:" + context.User.Identity.Name;
            }

            return base.GetVaryByCustomString(context, arg);
        }
    }
}
