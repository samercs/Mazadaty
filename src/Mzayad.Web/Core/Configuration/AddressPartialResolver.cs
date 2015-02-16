using System.Collections.Generic;

namespace Mzayad.Web.Core.Configuration
{
    public class AddressPartialResolver
    {
        /// <summary>
        /// Gets a country-specific view name for the Address.cshtml view.
        /// </summary>
        public static string GetViewName(string countryCode)
        {
            const string defaultViewName = "Address_Default";
            string viewName;

            var views = new Dictionary<string, string>
            {
                {"kw", "Address_KW"},
                {"us", "Address_US"}
            };

            viewName = views.TryGetValue(countryCode.ToLowerInvariant(), out viewName) ? viewName : defaultViewName;

            return string.Format("~/Views/Shared/EditorTemplates/{0}.cshtml", viewName);
        }
    }
}