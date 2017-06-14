using System;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;

namespace Mzayad.Web.Core.Services
{
    public class HttpContextService : IHttpContextService
    {
        private readonly HttpContextBase _httpContext;

        public HttpContextService(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }

        [Obsolete]
        public bool IsAuthenticated()
        {
            return _httpContext.Request.IsAuthenticated;
        }

        public bool IsLocal()
        {
            return _httpContext.Request.IsLocal;
        }

        public string GetAnonymousId()
        {
            return _httpContext.Request.AnonymousID;
        }

        [Obsolete]
        public string GetUserId()
        {
            return _httpContext.User.Identity.GetUserId();
        }

        public string GetUserName()
        {
            return _httpContext.User.Identity.GetUserName();
        }

        public string GetUserHostAddress()
        {
            return _httpContext.Request.UserHostAddress;
        }

        public string GetRequestParams()
        {
            var parameters = _httpContext.Request.Params.AllKeys
                .ToDictionary(i => i, i => _httpContext.Request.Params[i]);

            return JsonConvert.SerializeObject(parameters);
        }

        public string GetUrlScheme()
        {
            return _httpContext.Request.IsLocal ? "http" : "https";
        }
    }
}