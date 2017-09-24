using System;

namespace Mazadaty.Core.Extensions
{
    public static class UriExtensions
    {
        /// <summary>
        /// Gets the original URI with a new hostname and port suppressed.
        /// </summary>
        /// <remarks>
        /// Can be used to easily convert an Azure blob storage URL to an Azure CDN URL.
        /// </remarks>
        public static Uri WithHost(this Uri uri, string host)
        {
            var uriBuilder = new UriBuilder(uri)
            {
                Host = host,
                Port = -1
            };

            return uriBuilder.Uri;
        }
    }
}
