using System;
using System.Threading;
using System.Web.Mvc;
using Mzayad.Data;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Services.Client.Messaging;

namespace Mzayad.Web.Controllers
{
    public abstract class ApplicationController : Controller
    {
        protected string LanguageCode { get; set; }
        
        protected readonly IDataContextFactory DataContextFactory;
        protected readonly IAppSettings AppSettings;
        protected readonly IAuthService AuthService;
        protected readonly ICookieService CookieService;
        protected readonly IMessageService MessageService;
        
        protected ApplicationController(IControllerServices controllerServices)
        {
            DataContextFactory = controllerServices.DataContextFactory;
            AppSettings = controllerServices.AppSettings;
            AuthService = controllerServices.AuthService;
            CookieService = controllerServices.CookieService;
            MessageService = controllerServices.MessageService;
        }

        //protected override void ExecuteCore()
        //{
        //    RouteData.Values["languageCode"] = "ar";
            
        //    base.ExecuteCore();
        //}

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var languagecode = filterContext.RouteData.Values["languageCode"] ?? GetLanguageCode();
            ViewBag.LanguageCode = languagecode.ToString();
            LanguageCode = languagecode.ToString();

            base.OnActionExecuting(filterContext);
        }

        private static string GetLanguageCode()
        {
            var languageCode = "en";
            if (Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar", StringComparison.OrdinalIgnoreCase))
            {
                languageCode = "ar";
            }

            return languageCode;
        }
    }
}