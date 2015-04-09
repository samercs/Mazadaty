using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Services;
using Mzayad.Web.Areas.admin.Models.Avatar;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Services.Client.Storage;

namespace Mzayad.Web.Areas.admin.Controllers
{
    public class AvatarsController : ApplicationController
    {

        private readonly AvatarService _avatarService;
        private readonly IStorageService _storageService;

        public AvatarsController(IAppServices appServices,IStorageService storageService) : base(appServices)
        {
            _avatarService=new AvatarService(appServices.DataContextFactory);
            _storageService = storageService;
        }

        public async Task<ActionResult> Index()
        {
            var model = await _avatarService.GetAll();
            return View(model);
        }

        public async Task<ActionResult> Add()
        {
            var model = new AvatarAddViewModel().Init();
            return View(model);
        }


        [HttpPost,ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AvatarAddViewModel model, HttpPostedFileBase file)
        {
            if (file == null)
            {
                return Content("null1");
            }
            if (file.ContentLength == 0)
            {
                return Content("null2");
            }

            int[] avatarWidth = new[] {128};
            var url = await UploadImage(file, avatarWidth);
            model.Avatar.Url = url[0].AbsolutePath;
            await _avatarService.Add(model.Avatar);
            return RedirectToAction("Index");
        }

        private async Task<Uri[]> UploadImage(HttpPostedFileBase file, int[] widths)
        {
            if (file == null || file.ContentLength == 0 || !file.ContentType.Contains("image"))
            {
                throw new ArgumentException("Please only upload an image.");
            }

            var imageSettings = new ImageSettings
            {
                BackgroundColor = Color.White,
                ForceSquare = true,
                Widths = widths
            };

            var uris = await _storageService.SaveImage("avatars", file, imageSettings);
            if (uris.Length != widths.Length)
            {
                throw new Exception("Could not upload image, image service did not return expected results.");
            }

            return uris;
        }
    }
}