using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.SignalR;
using Autofac.Integration.WebApi;
using Microsoft.AspNet.SignalR;
using Mzayad.Data;
using Mzayad.Services.Activity;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Base.Web.Caching;
using OrangeJetpack.Cms.Client;
using OrangeJetpack.Services.Client.Messaging;
using OrangeJetpack.Services.Client.Storage;
using System;
using System.Configuration;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;

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

            builder.Register<IAppSettings>(c => new AppSettings(ConfigurationManager.AppSettings));

            builder.RegisterType<AppServices>().As<IAppServices>();
            builder.RegisterType<DataContextFactory>().As<IDataContextFactory>();
            builder.RegisterType<AuthService>().As<IAuthService>();
            builder.RegisterType<CookieService>().As<ICookieService>();
            builder.RegisterType<RequestService>().As<IRequestService>();


            builder.Register<ICmsClient>(i =>
            {
                var appSettings = i.Resolve<IAppSettings>();
                return new CmsClient(appSettings.ProjectKey, appSettings.ProjectToken);
            });

            builder.Register<IStorageService>(c =>
            {
                var appSettings = c.Resolve<IAppSettings>();
                return new AzureBlobService(appSettings.ProjectKey, appSettings.ProjectToken);
            });

            builder.Register<IMessageService>(c => new MessageService(c.Resolve<IAppSettings>().EmailSettings));
            builder.Register<IActivityQueueService>(c => new ActivityQueueService(ConfigurationManager.ConnectionStrings["QueueConnection"].ConnectionString));

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
            var hostName = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
            if (string.IsNullOrEmpty(hostName))
            {
                return new HttpCacheService();
            }

            return new RedisCacheService();
        }
    }
}