using System;

namespace Mazadaty.Core.Formatting
{
    public static class UrlFormatter
    {
        public const string CdnHost = "az723232.vo.msecnd.net";
        
        /// <summary>
        /// Changes a URI's host to the Azure CDN host.
        /// </summary>
        public static string GetCdnUrl(Uri uri)
        {
            var uriBuilder = new UriBuilder(uri)
            {
                Host = CdnHost, 
                Scheme = "https",
                Port = -1
            };

            return uriBuilder.ToString();
        }
    }
}
