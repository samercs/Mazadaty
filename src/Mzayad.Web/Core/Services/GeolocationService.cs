using MaxMind.GeoIP2.Model;

namespace Mzayad.Web.Core.Services
{
    public class GeolocationService : IGeolocationService
    {
        public Country GetCountry(string ipAddress)
        {
            var client = new MaxMind.GeoIP2.WebServiceClient(92956, "CBZpUlhIzp3X");
            return client.Country(ipAddress).Country;          
        }
    }
}