using OrangeJetpack.Services.Client.Messaging;
using System.Collections.Specialized;

namespace Mzayad.Web.Core.Services
{
    public class AppSettings : IAppSettings
    {       
        public EmailSettings EmailSettings { get; private set; }
        public string SiteName { get; private set; }
        public string CacheConnection { get; private set; }
        public decimal LocalShipping { get; private set; }
        public string AzureCdnUrlHost { get; private set; }
        public string ProjectKey { get; private set; }
        public string ProjectToken { get; private set; }

        public AppSettings(NameValueCollection appSettings)
        {
            SiteName = appSettings["SiteName"];
            CacheConnection = appSettings["CacheConnection"];
            AzureCdnUrlHost = appSettings["AzureCdnUrlHost"];

            ProjectKey = appSettings["OrangeJetpack.Services:Project.Key"];
            ProjectToken = appSettings["OrangeJetpack.Services:Project.Token"];
            
            EmailSettings = new EmailSettings
            {
                ProjectKey = ProjectKey,
                ProjectToken = ProjectToken,
                SenderAddress = "noreply@mzayad.com",
                SenderName = "Mzayad"
            };

            LocalShipping = decimal.Parse(appSettings["LocalShipping"]);
        }     
    }
}