namespace Mazadaty.Web.Core.Services
{
    public interface IHttpContextService
    {
        bool IsAuthenticated();
        bool IsLocal();
        string GetAnonymousId();
        string GetUserId();
        string GetUserName();
        string GetUserHostAddress();
        string GetRequestParams();
        string GetUrlScheme();
    }
}
