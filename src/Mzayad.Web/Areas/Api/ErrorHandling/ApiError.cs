using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mzayad.Web.Areas.Api.ErrorHandling
{
    public class ApiError
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ApiErrorType Type { get; set; }
        public string Message { get; set; }
        public dynamic Metadata { get; set; }
    }
}