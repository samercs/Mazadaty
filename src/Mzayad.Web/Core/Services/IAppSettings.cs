using Mzayad.Services.Messaging;

namespace Mzayad.Web.Core.Services
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