using OrangeJetpack.Services.Client.Messaging;

namespace Mzayad.Web.Core.Services
{
    public interface IAppSettings
    {
        EmailSettings EmailSettings { get; }
        string SiteName { get; }
        string CacheConnection { get; }
        string StorageConnection { get; }
        decimal LocalShipping { get; }
        string AzureCdnUrlHost { get; }
        string ProjectKey { get; }
        string ProjectToken { get; }
    }
}