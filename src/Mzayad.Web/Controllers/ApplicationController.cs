using Mzayad.Data;
using Mzayad.Services;
using Mzayad.Web.Core.Services;
using Mzayad.Web.Models.Shared;
using OrangeJetpack.Base.Web;
using OrangeJetpack.Services.Client.Messaging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Web.Mvc;
using Mzayad.Web.Core.ActionResults;
using Mzayad.Web.Core.Caching;
using OrangeJetpack.Base.Core.Extensions;
using OrangeJetpack.Services.Client.Storage;

namespace Mzayad.Web.Controllers
{
    public abstract class ApplicationController : Controller
    {
        protected string Language { get; set; }
        
        protected readonly IDataContextFactory DataContextFactory;
        protected readonly IAppSettings AppSettings;
        protected readonly IAuthService AuthService;
        protected readonly ICookieService CookieService;
        protected readonly IMessageService MessageService;
        protected readonly ICacheService CacheService;
        protected readonly IRequestService RequestService;
        protected readonly IStorageService StorageService;
        protected readonly EmailTemplateService EmailTemplateService;
        
        protected ApplicationController(IAppServices appServices)
        {
            DataContextFactory = appServices.DataContextFactory;
            AppSettings = appServices.AppSettings;
            AuthService = appServices.AuthService;
            CookieService = appServices.CookieService;
            MessageService = appServices.MessageService;
            CacheService = appServices.CacheService;
            RequestService = appServices.RequestService;
            StorageService = appServices.StorageService;
            
            EmailTemplateService=new EmailTemplateService(appServices.DataContextFactory);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Language = ViewBag.Language = GetLanguageCode();
            
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

        /// <summary>
        /// Sets a view status message for display.
        /// </summary>
        /// <param name="message">The message text to display.</param>
        /// <param name="statusMessageType">The <see cref="StatusMessageType"/> to use.</param>
        /// <param name="statusMessageFormat">The <see cref="StatusMessageFormat"/> to use.</param>
        public void SetStatusMessage(string message, StatusMessageType statusMessageType = StatusMessageType.Success, StatusMessageFormat statusMessageFormat = StatusMessageFormat.Normal)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            TempData["StatusMessage"] = new StatusMessage(message, statusMessageType, statusMessageFormat, false);
        }

        /// <summary>
        /// Sets a success view status message for display.
        /// </summary>
        /// <param name="message">A composite format message string to display.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void SetStatusMessage(string message, params object[] args)
        {
            message = string.Format(message, args);

            SetStatusMessage(message);
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

        /// <summary>
        /// Gets a ViewResult for Error with the supplied error message applied.
        /// </summary>
        protected ActionResult Error(string errorMessage)
        {
            SetStatusMessage(errorMessage, StatusMessageType.Error);
            return View("Blank");
        }

        /// <summary>
        /// Gets a RedirectResult for a return URL if URL is local else for Home.
        /// </summary>
        protected ActionResult RedirectToLocal(string url)
        {
            if (string.IsNullOrWhiteSpace(url) || !IsLocalUrl(url))
            {
                return RedirectToAction("Index", "Home");
            }

            return Redirect(url);
        }

        private bool IsLocalUrl(string url)
        {
            if (AuthService.IsLocal())
            {
                return true;
            }

            return true; // Url != null && Url.IsLocalUrl(url);
        }

        protected ExcelResult Excel<T>(IEnumerable<T> items, string fileName)
        {
            return new ExcelResult(items.ToDataTable(), fileName);
        }

        protected JsonResult JsonError(string error, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            Response.StatusCode = (int)statusCode;

            return Json(new
            {
                success = false,
                error
            });
        }

        public ActionResult DeleteConfirmation(string pageTitle, string confirmationMessage)
        {
            return View("DeleteConfirmation", new DeleteConfirmationViewModel
            {
                PageTitle = pageTitle,
                ConfirmationMessage = confirmationMessage
            });
        }
    }
}