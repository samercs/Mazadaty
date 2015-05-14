using System.Configuration;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Autofac.Integration.SignalR;
using Microsoft.AspNet.SignalR;
using Mzayad.Data;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Services.Client.Messaging;
using OrangeJetpack.Services.Client.Storage;

namespace Mzayad.Web
{
    public class AutofacConfig
    {
        public static IContainer RegisterAll()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacWebTypesModule());
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterHubs(Assembly.GetExecutingAssembly());

            builder.RegisterType<AppServices>().As<IAppServices>();
            builder.RegisterType<DataContextFactory>().As<IDataContextFactory>();
            builder.RegisterType<AuthService>().As<IAuthService>();
            builder.RegisterType<CookieService>().As<ICookieService>();
            builder.RegisterType<RequestService>().As<IRequestService>();
            builder.RegisterType<GeolocationService>().As<IGeolocationService>();
            builder.RegisterType<AzureBlobService>().As<IStorageService>();
            
            builder.Register<IAppSettings>(c => new AppSettings(ConfigurationManager.AppSettings));
            builder.Register<IMessageService>(c => new EmailService(c.Resolve<IAppSettings>().EmailSettings));

            builder.Register(GetCacheService).SingleInstance();

            return Container(builder);
        }

        private static IContainer Container(ContainerBuilder builder)
        {
            var container = builder.Build();
            DependencyResolver.SetResolver(new Autofac.Integration.Mvc.AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            GlobalHost.DependencyResolver = new Autofac.Integration.SignalR.AutofacDependencyResolver(container);
            
            return container;
        }

        /// <remarks>
        /// Use standard HttpContext caching when running locally/debug, use Redis in Azure.
        /// </remarks>
        private static ICacheService GetCacheService(IComponentContext c)
        {
            return new HttpCacheService();


#if DEBUG
            return new HttpCacheService();
#else           
            var connectionString = c.Resolve<IAppSettings>().CacheConnection;
            var cacheKeyPrefix = "mz";
            return new RedisCacheService(connectionString, cacheKeyPrefix);
#endif
        }
    }
}