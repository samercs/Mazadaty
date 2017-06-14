using System;
using System.Linq;
using System.Web;

namespace Mzayad.Web.Extensions
{
    public static class RequestExtensions
    {
        public static bool IsSecureOrTerminatedSecureConnection(this HttpRequest request)
        {
            if (!request.IsSslTerminated())
            {
                return request.IsSecureConnection;
            }

            var header = request.Headers["X-Forwarded-Proto"];
            return string.Equals(header, "https", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsSslTerminated(this HttpRequest request)
        {
            return request.Headers.AllKeys.Contains("X-Forwarded-Proto");
        }
    }
}