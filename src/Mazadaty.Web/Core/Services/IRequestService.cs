namespace Mazadaty.Web.Core.Services
{
    public interface IRequestService
    {
        string GetRequestParams();
        string GetUrlScheme();
        string GetHostAddress();
    }
}
