using Mzayad.Data;
using OrangeJetpack.Services.Client.Messaging;

namespace Mzayad.Web.Core.Services
{
    public interface IAppServices
    {
        IDataContextFactory DataContextFactory { get; }
        IAppSettings AppSettings { get; }
        IAuthService AuthService { get; }
        ICookieService CookieService { get; }
        IMessageService MessageService { get; }
        IGeolocationService GeolocationService { get; }
        ICacheService CacheService { get; }
    }
}