using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Mzayad.Client
{
    internal static class HttpClientFactory
    {
        private const string BaseUrl = "https://mzayad.orangejetpack.com/api/";

        public static HttpClient Create()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }
    }
}
