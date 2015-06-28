using Mzayad.Data;
using Mzayad.Services;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Services.Client.Messaging;
using OrangeJetpack.Services.Client.Storage;
using System.Web.Http;

namespace Mzayad.Web.Controllers
{
    public abstract class ApplicationApiController : ApiController
    {
        protected readonly IDataContextFactory DataContextFactory;
        protected readonly IAppSettings AppSettings;
        protected readonly IAuthService AuthService;
        protected readonly ICookieService CookieService;
        protected readonly IMessageService MessageService;
        protected readonly IGeolocationService GeolocationService;
        protected readonly ICacheService CacheService;
        protected readonly IRequestService RequestService;
        protected readonly IStorageService StorageService;
        protected readonly EmailTemplateService EmailTemplateService;

        protected ApplicationApiController(IAppServices appServices)
        {
            DataContextFactory = appServices.DataContextFactory;
            AppSettings = appServices.AppSettings;
            AuthService = appServices.AuthService;
            CookieService = appServices.CookieService;
            MessageService = appServices.MessageService;
            GeolocationService = appServices.GeolocationService;
            CacheService = appServices.CacheService;
            RequestService = appServices.RequestService;
            StorageService = appServices.StorageService;

            EmailTemplateService = new EmailTemplateService(appServices.DataContextFactory);
        }
    }
}