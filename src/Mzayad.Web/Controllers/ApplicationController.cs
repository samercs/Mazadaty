using System.Web.Mvc;
using Mzayad.Data;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Services.Client.Messaging;

namespace Mzayad.Web.Controllers
{
    public abstract class ApplicationController : Controller
    {
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
    }
}