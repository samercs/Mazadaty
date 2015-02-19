﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;

namespace Mzayad.Web.Areas.admin.Controllers
{
    public class ReportsController : ApplicationController
    {
        //
        // GET: /admin/Reports/
        public ReportsController(IControllerServices controllerServices) : base(controllerServices)
        {
        }

        public ActionResult Index()
        {
            return View();
        }
	}
}