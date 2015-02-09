using Mzayad.Data;
using OrangeJetpack.Services.Client.Messaging;

namespace Mzayad.Web.Core.Services
{
    public class ControllerServices : IControllerServices
    {
        public IDataContextFactory DataContextFactory { get; private set; }
        public IAppSettings AppSettings { get; private set; }
        public IAuthService AuthService { get; private set; }
        public ICookieService CookieService { get; private set; }
        public IMessageService MessageService { get; private set; }

        public ControllerServices(
            IDataContextFactory dataContextFactory,
            IAppSettings appSettings,
            IAuthService authService,
            ICookieService cookieService,
            IMessageService messageService)
        {
            DataContextFactory = dataContextFactory;
            AppSettings = appSettings;
            AuthService = authService;
            CookieService = cookieService;
            MessageService = messageService;
        }
    }
}