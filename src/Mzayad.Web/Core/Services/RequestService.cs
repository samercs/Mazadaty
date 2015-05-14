using Newtonsoft.Json;
using System.Linq;
using System.Web;

namespace Mzayad.Web.Core.Services
{
    public class RequestService : IRequestService
    {
        private readonly HttpContextBase _httpContext;

        public RequestService(HttpContextBase httpContext)
        {
            _httpContext = httpContext;
        }
        
        public string GetRequestParams()
        {
            var parameters = _httpContext.Request.Params.AllKeys
                .ToDictionary(i => i, i => _httpContext.Request.Params[i]);

            return JsonConvert.SerializeObject(parameters);
        }

        public string GetUrlScheme()
        {
            return _httpContext.Request.Url == null ? "http" : _httpContext.Request.Url.Scheme;
        }
    }
}
