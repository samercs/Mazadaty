using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mazadaty.Web.Areas.Api.ErrorHandling
{
    public class ApiErrorResult : IHttpActionResult
    {
        private readonly ApiError _apiError;

        public ApiErrorResult(ApiError apiError)
        {
            _apiError = apiError;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(new
                {
                    error = _apiError
                }, new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                }))
            };
            return Task.FromResult(response);
        }
    }
}
