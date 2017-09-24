using Mazadaty.Services.Messaging;

namespace Mazadaty.Web.Core.Services
{
    public interface IAppSettings
    {
        EmailSettings EmailSettings { get; }
        string SiteName { get; }
        string CacheConnection { get; }
        decimal LocalShipping { get; }
        string CanonicalUrl { get; }
    }
}
