
using System.Collections.Specialized;
using Mzayad.Services.Messaging;

namespace Mzayad.Web.Core.Services
{
    public class AppSettings : IAppSettings
    {
        public EmailSettings EmailSettings { get; private set; }
        public string SiteName { get; private set; }
        public string CacheConnection { get; private set; }
        public string StorageConnection { get; private set; }
        public decimal LocalShipping { get; private set; }
        
        public string CanonicalUrl { get; private set; }

        public AppSettings(NameValueCollection appSettings)
        {
            SiteName = appSettings["SiteName"];
            CacheConnection = appSettings["CacheConnection"];
            StorageConnection = appSettings["StorageConnection"];
            CanonicalUrl = appSettings["CanonicalUrl"];
            LocalShipping = decimal.Parse(appSettings["LocalShipping"]);
            EmailSettings = new EmailSettings
            {
                FromEmail = appSettings["FromEmail"],
                FromName = appSettings["FromName"],
                EmailPassword = appSettings["EmailPassword"]
            };
        }
    }
}