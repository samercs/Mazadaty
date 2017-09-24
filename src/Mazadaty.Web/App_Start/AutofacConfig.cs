using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.SignalR;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.SignalR;
using Mazadaty.Data;
using Mazadaty.Web.Core.Services;
using OrangeJetpack.Services.Client.Storage;
using System.Configuration;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Mazadaty.Services.Caching;
using Mazadaty.Services.Messaging;
using Mazadaty.Services.Queues;
using Mazadaty.Web.Core.ShoppingCart;


namespace Mazadaty.Web
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

            builder.Register<IAppSettings>(c => new AppSettings(ConfigurationManager.AppSettings));

            builder.RegisterType<AppServices>().As<IAppServices>();
            builder.RegisterType<DataContextFactory>().As<IDataContextFactory>();
            builder.RegisterType<AuthService>().As<IAuthService>();
            builder.RegisterType<CookieService>().As<ICookieService>();
            builder.RegisterType<RequestService>().As<IRequestService>();
            builder.RegisterType<HttpContextService>().As<IHttpContextService>().InstancePerRequest();


           builder.Register<IStorageService>(c =>
            {
                var appSettings = c.Resolve<IAppSettings>();
                return new AzureBlobService("", "");
            });

            builder.Register<IMessageService>(c => new EmailMessageService(c.Resolve<IAppSettings>().EmailSettings));
            builder.Register<IQueueService>(c => new QueueService(ConfigurationManager.ConnectionStrings["QueueConnection"].ConnectionString));


            builder.Register(GetCacheService).SingleInstance();
            builder.RegisterType<CartService>().As<ICartService>().InstancePerRequest();
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
        private static ICachService GetCacheService(IComponentContext c)
        {
            return new HttpCacheService();
        }

        //private static ICartService GetCartService(IComponentContext c)
        //{
        //    var userService = c.Resolve<IAuthService>();
        //    var cacheService = c.Resolve<ICacheService>();
        //    return new CartService(userService, cacheService);
        //}
    }
}
