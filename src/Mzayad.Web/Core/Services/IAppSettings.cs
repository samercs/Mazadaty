using OrangeJetpack.Services.Client.Messaging;

namespace Mzayad.Web.Core.Services
{
    public interface IAppSettings
    {
        EmailSettings EmailSettings { get; }
        string SiteName { get; }
        string CacheConnection { get; }
    }
}