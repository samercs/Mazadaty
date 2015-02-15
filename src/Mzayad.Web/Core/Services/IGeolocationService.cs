using MaxMind.GeoIP2.Model;

namespace Mzayad.Web.Core.Services
{
    public interface IGeolocationService
    {
        Country GetCountry(string ipAddress);
    }
}
