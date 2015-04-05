using OrangeJetpack.Services.Client.Messaging;
using System.Collections.Specialized;

namespace Mzayad.Web.Core.Services
{
    public class AppSettings : IAppSettings
    {       
        public EmailSettings EmailSettings { get; private set; }
        public string SiteName { get; private set; }
        public string CacheConnection { get; private set; }
        
        public AppSettings(NameValueCollection appSettings)
        {
            EmailSettings = new EmailSettings
            {
                ProjectKey = appSettings["OrangeJetpack.Services:Project.Key"],
                ProjectToken = appSettings["OrangeJetpack.Services:Project.Token"],
                SenderAddress = "noreply@mzayad.com",
                SenderName = "Mzayad"
            };

            SiteName = appSettings["SiteName"];
            CacheConnection = appSettings["CacheConnection"];
        }     
    }
}