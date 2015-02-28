using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Mzayad.Api.Tests
{
    class Program
    {
        static void Main()
        {
            RunAsync().Wait();
        }

        static async Task RunAsync()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44300/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var requestContent = new FormUrlEncodedContent(new[] 
                {
                    new KeyValuePair<string, string>("grant_type", "password"),
                    new KeyValuePair<string, string>("username", "andy.mehalick"),
                    new KeyValuePair<string, string>("password", "asdfasdf"),
                });

                var reponseMessage = await client.PostAsync("token", requestContent);
                var responseContent = await reponseMessage.Content.ReadAsStringAsync();
                var responseModel = JsonConvert.DeserializeObject<TokenResponseModel>(responseContent);
                var accessToken = responseModel.AccessToken;

                // add token to authorization header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
                var response = await client.GetStringAsync("api/test1");

                Console.WriteLine(response);
                
                Console.ReadLine();
            }
        }
    }
}
