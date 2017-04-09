using Microsoft.AspNet.Identity;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.ModelBinder;
using Mzayad.Web.Extensions;
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
            JsonConfig.Configure();
        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }

        protected void Application_BeginRequest()
        {
            // to prevent preflight OPTION request sent by AngularJS from returning error
            if (Request.Headers.AllKeys.Contains("Origin", StringComparer.OrdinalIgnoreCase) && Request.HttpMethod.Equals("OPTIONS"))
            {
                Response.Flush();
                Response.End();
            }

            if (Request.IsLocal || Request.IsSecureOrTerminatedSecureConnection())
            {
                return;
            }

            RedirectToCanonicalUrl(Request.Url);
        }

        private void RedirectToCanonicalUrl(Uri uri)
        {
            var uriBuilder = new UriBuilder(uri)
            {
                //Host = AppSettings.CanonicalUrl,
                Scheme = "https",
                Port = -1
            };

            Response.Redirect(uriBuilder.ToString());
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            var cultureInfo = GetCultureFromRoute() ?? GetCultureFromCookie() ?? GetCultureFromBrowser();

            cultureInfo.DateTimeFormat.Calendar = new GregorianCalendar();
            cultureInfo.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";

            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
            {
                return;
            }

            var sessionLog = new SessionLogService(new DataContextFactory());
            try
            {
                sessionLog.Insert(new SessionLog
                {
                    Browser = Request.UserAgent,
                    IP = Request.UserHostAddress,
                    UserId = HttpContext.Current.User.Identity.GetUserId()
                });
            }
            catch
            {
                // do nothing
            }
        }

        private CultureInfo GetCultureFromRoute()
        {
            var handler = Context.Handler as MvcHandler;
            var routeData = handler?.RequestContext.RouteData;
            var languageRoute = routeData?.Values["language"];
            return languageRoute == null ? null : CultureInfo.CreateSpecificCulture(languageRoute.ToString());
        }

        private static CultureInfo GetCultureFromCookie()
        {
            var languageCookie = HttpContext.Current.Request.Cookies.Get(CookieKeys.LanguageCode);
            return languageCookie == null ? null : CultureInfo.CreateSpecificCulture(languageCookie.Value);
        }

        private static CultureInfo GetCultureFromBrowser()
        {
            var languagePreference = HttpContext.Current.Request.UserLanguages?.FirstOrDefault() ?? "";
            if (languagePreference.StartsWith("ar", StringComparison.OrdinalIgnoreCase))
            {
                return CultureInfo.CreateSpecificCulture("ar");
            }

            return CultureInfo.CreateSpecificCulture("en");
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
