using System.Configuration;
using System.Web.Mvc;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Base.Core.Formatting;

namespace Mzayad.Web.Core.Configuration
{
    public abstract class CustomViewPage : WebViewPage
    {
        private IAppSettings _appSettings;
        public IAppSettings AppSettings
        {
            get
            {
                return (_appSettings = _appSettings ?? new AppSettings(ConfigurationManager.AppSettings));
            }
        }

        public string LanguageCode
        {
            get { return ViewBag.LanguageCode ?? "en"; }
        }

        /// <summary>
        /// Gets a formatting string from OrangeJetpack.Base.Core.Formatting.StringFormatter.ObjectFormat();
        /// </summary>
        public MvcHtmlString F(string format, object source)
        {
            return MvcHtmlString.Create(StringFormatter.ObjectFormat(format, source));
        }
    }

    public abstract class CustomViewPage<TModel> : WebViewPage<TModel>
    {
        private IAppSettings _appSettings;
        public IAppSettings AppSettings
        {
            get
            {
                return (_appSettings = _appSettings ?? new AppSettings(ConfigurationManager.AppSettings));
            }
        }

        public string LanguageCode
        {
            get { return ViewBag.Language ?? "en"; }
        }

        public string SiteTitle
        {
            get { return ConfigurationManager.AppSettings["SiteName"]; }
        }

        /// <summary>
        /// Gets a formatting string from OrangeJetpack.Base.Core.Formatting.StringFormatter.ObjectFormat();
        /// </summary>
        public MvcHtmlString F(string format, object source)
        {
            return MvcHtmlString.Create(StringFormatter.ObjectFormat(format, source));
        }
    }
}
