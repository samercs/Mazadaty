using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Mzayad.Data;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Services.Client.Messaging;

namespace Mzayad.Web
{
    public class AutofacConfig
    {
        public static IContainer RegisterAll()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacWebTypesModule());
            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<ControllerServices>().As<IControllerServices>();
            builder.RegisterType<DataContextFactory>().As<IDataContextFactory>();
            builder.RegisterType<AuthService>().As<IAuthService>();
            builder.RegisterType<CookieService>().As<ICookieService>();
            builder.RegisterType<GeolocationService>().As<IGeolocationService>();
            
            builder.Register<IAppSettings>(c => new AppSettings(ConfigurationManager.AppSettings));
            builder.Register<IMessageService>(c => new EmailService(c.Resolve<IAppSettings>().EmailSettings));

            return Container(builder);
        }

        private static IContainer Container(ContainerBuilder builder)
        {
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            return container;
        }
    }
}