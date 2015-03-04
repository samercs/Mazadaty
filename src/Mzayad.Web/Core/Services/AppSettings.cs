using System.Collections.Specialized;
using System.Configuration;
using OrangeJetpack.Services.Client.Messaging;

namespace Mzayad.Web.Core.Services
{
    public class AppSettings : IAppSettings
    {
        public EmailSettings EmailSettings { get; private set; }
        
        public AppSettings(NameValueCollection appSettings)
        {
            EmailSettings = new EmailSettings
            {
                ProjectKey = appSettings["OrangeJetpack.Services:Project.Key"],
                ProjectToken = appSettings["OrangeJetpack.Services:Project.Token"],
                SenderAddress = "noreply@mzayad.com",
                SenderName = "Mzayad"
            };
        }

        public string SiteName
        {
            get { return ConfigurationManager.AppSettings["SiteName"]; }
        }

       
    }
}