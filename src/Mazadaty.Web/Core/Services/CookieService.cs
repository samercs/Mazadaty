using System;
using System.Web;

namespace Mazadaty.Web.Core.Services
{
    public class CookieService : ICookieService
    {
        private readonly HttpRequestBase _httpRequestBase;
        private readonly HttpResponseBase _httpResponseBase;

        public CookieService(HttpContextBase httpContext)
        {
            _httpRequestBase = httpContext.Request;
            _httpResponseBase = httpContext.Response;
        }

        public string Get(string name)
        {
            return _httpRequestBase.Cookies[name] == null ? null : _httpRequestBase.Cookies[name].Value;
        }

        public bool TryGetBool(string name, bool defaultValue = false)
        {
            if (_httpRequestBase.Cookies[name] == null)
            {
                return defaultValue;
            }

            bool output;
            return bool.TryParse(_httpRequestBase.Cookies[name].Value, out output) ? output : defaultValue;
        }

        public void Add(string name, object value, DateTime? expiration = null, string path = null)
        {
            Add(name, value.ToString(), expiration, path);
        }

        public void Add(string name, string value, DateTime? expiration = null, string path = null)
        {
            var cookie = new HttpCookie(name)
            {
                Value = value,
                Secure = _httpRequestBase.IsSecureConnection,
                HttpOnly = true
            };

            if (expiration.HasValue)
            {
                cookie.Expires = expiration.Value;
            }

            if (!string.IsNullOrEmpty(path))
            {
                cookie.Path = path;
            }
            
            _httpResponseBase.AppendCookie(cookie);
        }
    }
}
