using OrangeJetpack.Services.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace Mzayad.Client
{
    public class EmailClient
    {
        private readonly HttpClient _httpClient;
        private readonly Email _email;

        public EmailClient(Email email)
        {
            _httpClient = HttpClientFactory.Create();
            _email = email;
        }

        public async Task Send()
        {
            await _httpClient.PostAsJsonAsync("messages", _email);
        }
    }
}
