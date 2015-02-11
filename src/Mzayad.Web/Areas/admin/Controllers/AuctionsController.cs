using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;

namespace Mzayad.Web.Areas.admin.Controllers
{
    public class AuctionsController : ApplicationController
    {
        //
        // GET: /admin/Auctions/
        public AuctionsController(IControllerServices controllerServices) : base(controllerServices)
        {
        }

        public ActionResult Index()
        {
            return View();
        }
	}
}