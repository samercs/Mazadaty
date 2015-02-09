using Mzayad.Data;

namespace Mzayad.Web.Core.Services
{
    public interface IControllerServices
    {
        IDataContextFactory DataContextFactory { get; }
        IAuthService AuthService { get; }
        ICookieService CookieService { get; }
    }
}