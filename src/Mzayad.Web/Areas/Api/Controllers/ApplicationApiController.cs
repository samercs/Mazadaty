using System.Web.Http;
using Mzayad.Data;
using Mzayad.Services;
using Mzayad.Web.Areas.Api.Filters;
using Mzayad.Web.Core.Caching;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Services.Client.Messaging;
using OrangeJetpack.Services.Client.Storage;

namespace Mzayad.Web.Areas.Api.Controllers
{
    [LanguageFilter]
    public abstract class ApplicationApiController : ApiController
    {
        protected readonly IDataContextFactory DataContextFactory;
        protected readonly IAppSettings AppSettings;
        protected readonly IAuthService AuthService;
        protected readonly ICookieService CookieService;
        protected readonly IMessageService MessageService;
        protected readonly ICacheService CacheService;
        protected readonly IRequestService RequestService;
        protected readonly IStorageService StorageService;
        protected readonly EmailTemplateService EmailTemplateService;

        public string Language { get; set; }

        protected ApplicationApiController(IAppServices appServices)
        {
            DataContextFactory = appServices.DataContextFactory;
            AppSettings = appServices.AppSettings;
            AuthService = appServices.AuthService;
            CookieService = appServices.CookieService;
            MessageService = appServices.MessageService;
            CacheService = appServices.CacheService;
            RequestService = appServices.RequestService;
            StorageService = appServices.StorageService;

            EmailTemplateService = new EmailTemplateService(appServices.DataContextFactory);
        }
    }
}