using Mazadaty.Data;
using Mazadaty.Services.Caching;
using Mazadaty.Services.Messaging;
using Mazadaty.Services.Queues;


using OrangeJetpack.Cms.Client;

using OrangeJetpack.Services.Client.Storage;

namespace Mazadaty.Web.Core.Services
{
    public class AppServices : IAppServices
    {
        public IDataContextFactory DataContextFactory { get; }
        public IAppSettings AppSettings { get; }
        public IAuthService AuthService { get; }
        public ICookieService CookieService { get; }
        public IMessageService MessageService { get; }
        public ICachService CacheService { get; }
        public IRequestService RequestService { get; }
        public IStorageService StorageService { get; }
        public IQueueService QueueService { get; }
        public IHttpContextService HttpContextService { get; }

        public AppServices(
            IDataContextFactory dataContextFactory,
            IAppSettings appSettings,
            IAuthService authService,
            ICookieService cookieService,
            ICachService caceService,
            IRequestService requestService,
            IStorageService storageService,
            IQueueService queueService,
            IHttpContextService httpContextService,
            IMessageService messageService)
        {
            DataContextFactory = dataContextFactory;
            AppSettings = appSettings;
            AuthService = authService;
            CookieService = cookieService;
            CacheService = caceService;
            RequestService = requestService;
            StorageService = storageService;
            QueueService = queueService;
            HttpContextService = httpContextService;
            MessageService = messageService;
        }
    }
}
