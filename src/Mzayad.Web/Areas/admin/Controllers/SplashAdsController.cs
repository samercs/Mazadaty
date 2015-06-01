using System.Drawing;
using Mzayad.Services;
using Mzayad.Web.Areas.Admin.Models.SplashAds;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OrangeJetpack.Base.Web;
using OrangeJetpack.Services.Client.Storage;

namespace Mzayad.Web.Areas.admin.Controllers
{
    [RoleAuthorize(Role.Administrator)]
    [RouteArea("admin"), RoutePrefix("splash-ads")]
    public class SplashAdsController : ApplicationController
    {
        private readonly SplashAdService _splashAdService;
        
        public SplashAdsController(IAppServices appServices) : base(appServices)
        {
            _splashAdService = new SplashAdService(DataContextFactory);
        }

        [Route("")]
        public async Task<ActionResult> Index()
        {
            var viewModel = new IndexViewModel
            {
                SplashAds = (await _splashAdService.GetAll()).ToList()
            };

            return View(viewModel);
        }

        [Route("upload")]
        public ActionResult Upload()
        {
            return View();
        }

        [Route("upload")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Upload(HttpPostedFileBase file)
        {
            var imageSettings = new ImageSettings
            {
                Widths = new[] { 1080 },
                BackgroundColor = Color.White,
                ForceSquare = true
            };

            var urls = await StorageService.SaveImage("splash-ads", file, imageSettings);
            if (urls.Single() == null)
            {
                SetStatusMessage("We're sorry but we could not upload this ad, please check the file format and try again.", StatusMessageType.Warning);
                return RedirectToAction("Index");
            }

            var url = urls.Single();

            SetStatusMessage("Splash ad successfully uploaded.");

            return RedirectToAction("Index");
        }
    }
}