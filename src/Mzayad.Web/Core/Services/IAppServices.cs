using Mzayad.Data;
using OrangeJetpack.Base.Web.Caching;
using OrangeJetpack.Cms.Client;
using OrangeJetpack.Services.Client.Messaging;
using OrangeJetpack.Services.Client.Storage;

namespace Mzayad.Web.Core.Services
{
    public interface IAppServices
    {
        IDataContextFactory DataContextFactory { get; }
        IAppSettings AppSettings { get; }
        IAuthService AuthService { get; }
        ICookieService CookieService { get; }
        IMessageService MessageService { get; }
        ICacheService CacheService { get; }
        IRequestService RequestService { get; }
        IStorageService StorageService { get; }
        ICmsClient CmsClient { get; }
    }
}