using Mzayad.Data;
using Mzayad.Services.Activity;
using Mzayad.Services.Queues;
using OrangeJetpack.Base.Web.Caching;
using OrangeJetpack.Cms.Client;
using OrangeJetpack.Services.Client.Messaging;
using OrangeJetpack.Services.Client.Storage;

namespace Mzayad.Web.Core.Services
{
    public class AppServices : IAppServices
    {
        public IDataContextFactory DataContextFactory { get; }
        public IAppSettings AppSettings { get; }
        public IAuthService AuthService { get; }
        public ICookieService CookieService { get; }
        public IMessageService MessageService { get; }
        public ICacheService CacheService { get; }
        public IRequestService RequestService { get; }
        public IStorageService StorageService { get; }
        public ICmsClient CmsClient { get; }
        public IQueueService QueueService { get; }

        public AppServices(
            IDataContextFactory dataContextFactory,
            IAppSettings appSettings,
            IAuthService authService,
            ICookieService cookieService,
            IMessageService messageService,
            ICacheService caceService,
            IRequestService requestService,
            IStorageService storageService,
            ICmsClient cmsClient,
            IQueueService queueService)
        {
            DataContextFactory = dataContextFactory;
            AppSettings = appSettings;
            AuthService = authService;
            CookieService = cookieService;
            MessageService = messageService;
            CacheService = caceService;
            RequestService = requestService;
            StorageService = storageService;
            CmsClient = cmsClient;
            QueueService = queueService;
        }
    }
}