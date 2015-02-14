using System;
using System.Threading;
using System.Web.Mvc;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Base.Web;
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
        protected readonly EmailTemplateService _EmailTemplateService;
        
        protected ApplicationController(IControllerServices controllerServices)
        {
            DataContextFactory = controllerServices.DataContextFactory;
            AppSettings = controllerServices.AppSettings;
            AuthService = controllerServices.AuthService;
            CookieService = controllerServices.CookieService;
            MessageService = controllerServices.MessageService;
            _EmailTemplateService=new EmailTemplateService(controllerServices.DataContextFactory);
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


        public void SetStatusMessage(string message, StatusMessageType statusMessageType = StatusMessageType.Success, StatusMessageFormat statusMessageFormat = StatusMessageFormat.Normal)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            TempData["StatusMessage"] = new StatusMessage(message, statusMessageType, statusMessageFormat, false);
        }

        /// <summary>
        /// Gets a status message if one is set.
        /// </summary>
        public StatusMessage GetStatusMessage()
        {
            return TempData["StatusMessage"] as StatusMessage;
        }

        /// <summary>
        /// Gets a blank view with a status message.
        /// </summary>
        /// <param name="message">The message text to display.</param>
        /// <param name="statusMessageType">The <see cref="StatusMessageType"/> to use.</param>
        public ActionResult StatusMessage(string message, StatusMessageType statusMessageType = StatusMessageType.Success)
        {
            SetStatusMessage(message, statusMessageType);
            return View("Blank");
        }
    }
}