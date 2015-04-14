﻿using Mzayad.Data;
using OrangeJetpack.Services.Client.Messaging;

namespace Mzayad.Web.Core.Services
{
    public class AppServices : IAppServices
    {
        public IDataContextFactory DataContextFactory { get; private set; }
        public IAppSettings AppSettings { get; private set; }
        public IAuthService AuthService { get; private set; }
        public ICookieService CookieService { get; private set; }
        public IMessageService MessageService { get; private set; }
        public IGeolocationService GeolocationService { get; private set; }
        public ICacheService CacheService { get; private set; }

        public AppServices(
            IDataContextFactory dataContextFactory,
            IAppSettings appSettings,
            IAuthService authService,
            ICookieService cookieService,
            IMessageService messageService,
            IGeolocationService geolocationService,
            ICacheService caceService)
        {
            DataContextFactory = dataContextFactory;
            AppSettings = appSettings;
            AuthService = authService;
            CookieService = cookieService;
            MessageService = messageService;
            GeolocationService = geolocationService;
            CacheService = caceService;
        }
    }
}