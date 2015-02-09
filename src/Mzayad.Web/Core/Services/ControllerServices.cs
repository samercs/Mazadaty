using Mzayad.Data;

namespace Mzayad.Web.Core.Services
{
    public class ControllerServices : IControllerServices
    {
        public IDataContextFactory DataContextFactory { get; private set; }
        public IAuthService AuthService { get; private set; }
        public ICookieService CookieService { get; private set; }

        public ControllerServices(
            IDataContextFactory dataContextFactory,
            IAuthService authService,
            ICookieService cookieService)
        {
            DataContextFactory = dataContextFactory;
            AuthService = authService;
            CookieService = cookieService;
        }
    }
}