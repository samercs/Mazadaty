﻿using System.Web.Mvc;
using Mzayad.Data;
using Mzayad.Web.Core.Services;

namespace Mzayad.Web.Controllers
{
    public abstract class ApplicationController : Controller
    {
        protected readonly IDataContextFactory DataContextFactory;
        protected readonly IAuthService AuthService;
        protected readonly ICookieService CookieService;
        
        protected ApplicationController(IControllerServices controllerServices)
        {
            DataContextFactory = controllerServices.DataContextFactory;
            AuthService = controllerServices.AuthService;
            CookieService = controllerServices.CookieService;
        }
    }
}