using Mazadaty.Data;
using Mazadaty.Services.Caching;
using Mazadaty.Services.Messaging;
using Mazadaty.Services.Queues;

using OrangeJetpack.Cms.Client;
using OrangeJetpack.Services.Client.Storage;
namespace Mazadaty.Web.Core.Services
{
    public interface IAppServices
    {
        IDataContextFactory DataContextFactory { get; }
        IAppSettings AppSettings { get; }
        IAuthService AuthService { get; }
        ICookieService CookieService { get; }
        ICachService CacheService { get; }
        IRequestService RequestService { get; }
        IStorageService StorageService { get; }
        IQueueService QueueService { get; }
        IHttpContextService HttpContextService { get; }
        IMessageService MessageService { get; }
    }
}
