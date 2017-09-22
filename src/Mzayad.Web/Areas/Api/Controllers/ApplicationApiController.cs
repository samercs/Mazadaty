using Mzayad.Data;
using Mzayad.Services;
using Mzayad.Web.Areas.Api.ErrorHandling;
using Mzayad.Web.Areas.Api.Filters;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Base.Web.Caching;
using OrangeJetpack.Services.Client.Storage;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Mzayad.Services.Messaging;

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

        public IHttpActionResult ModelStateError(ModelStateDictionary modelState)
        {
            return new ApiErrorResult(new ApiError
            {
                Type = ApiErrorType.ModelStateError,
                Message = "Model state is invalid.",
                Metadata = new
                {
                    fields = modelState.Keys.ToArray()
                }
            });
        }

        protected IHttpActionResult InsufficientTokensError(object paramater = null)
        {
            return new ApiErrorResult(new ApiError
            {
                Type = ApiErrorType.InsufficientTokensError,
                Message = "Insufficient Tokens",
                Metadata = new
                {
                    paramater
                }
            });
        }

        protected IHttpActionResult ApiErroResult(string message, ApiErrorType type)
        {
            return new ApiErrorResult(new ApiError
            {
                Type = type,
                Message = message
            });
        }
    }
}