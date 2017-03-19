using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mzayad.Core.Extensions;
using Mzayad.Core.Formatting;
using Mzayad.Models;
using Mzayad.Models.Enums;
using Mzayad.Services;
using Mzayad.Web.Areas.Admin.Models.Banners;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Configuration;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Localization;
using OrangeJetpack.Services.Client.Storage;

namespace Mzayad.Web.Areas.Admin.Controllers
{
    [RouteArea("admin"), RoutePrefix("banners"), RoleAuthorize(Role.Administrator)]
    public class BannersController : ApplicationController
    {

        private readonly BannerService _bannerService;
        private readonly IStorageService _storageService;
        public BannersController(IAppServices appServices) : base(appServices)
        {
            _bannerService = new BannerService(DataContextFactory);
            _storageService = appServices.StorageService;
        }
        [Route("")]
        public async Task<ActionResult> Index(string search = null)
        {
            return View();
        }

        private async Task<IEnumerable<Banner>> GetBanners()
        {
            var banners = await _bannerService.GetAll();
            banners = banners.Localize(Language, LocalizationDepth.OneLevel);
            return banners;
        }

        public async Task<JsonResult> GetBanners([DataSourceRequest] DataSourceRequest request)
        {
            var result = await GetBanners();
            return Json(result.ToDataSourceResult(request));
        }
        [Route("add")]
        public ActionResult Add()
        {
            var model = new AddViewModel
            {
                Banner = new Banner
                {
                    Title = LocalizedContent.Init(),
                    SecondaryTitle = LocalizedContent.Init(),
                    Status = BannerStatus.Public
                }
            };
            return View(model);
        }

        [Route("add")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddViewModel model, LocalizedContent[] title,
            LocalizedContent[] secondaryTitle, HttpPostedFileBase bannerImage)
        {
            if (bannerImage.ContentLength != 0)
            {
                model.Banner.Title = title.Serialize();
                model.Banner.SecondaryTitle = secondaryTitle.Serialize();
                var orginalImageUri = await _storageService.SaveFile("banners", bannerImage);
                model.Banner.OrginalImgUrl = orginalImageUri.AbsoluteUri;
                var imagesUrl = await UploadImage(bannerImage,
                    new[]
                    {
                        ImageWidthConstants.BannerImageSm, ImageWidthConstants.BannerImageMd,
                        ImageWidthConstants.BannerImageMd
                    });
                model.Banner.ImgSmUrl = imagesUrl[0].AbsoluteUri;
                model.Banner.ImgMdUrl = imagesUrl[1].AbsoluteUri;
                model.Banner.ImgLgUrl = imagesUrl[2].AbsoluteUri;
                await _bannerService.Save(model.Banner);
                SetStatusMessage("Banner has been saved successfully.");
                return RedirectToAction("Index");
            }

            return Content("no file");
        }

        [Route("{bannerId:int}")]
        public async Task<ActionResult> Edit(int bannerId)
        {
            var banner = await _bannerService.GetById(bannerId);
            if (banner == null)
            {
                return HttpNotFound();
            }
            var model = new AddViewModel
            {
                Banner = banner
            };
            return View(model);
        }

        [Route("{bannerId:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int bannerId, AddViewModel model, LocalizedContent[] title, LocalizedContent[] secondaryTitle)
        {
            var banner = await _bannerService.GetById(bannerId);
            if (banner == null)
            {
                return HttpNotFound();
            }
            banner.Title = title.Serialize();
            banner.SecondaryTitle = secondaryTitle.Serialize();
            banner.Status = model.Banner.Status;
            banner.Url = model.Banner.Url;
            
            await _bannerService.Update(banner);
            SetStatusMessage("Banner has been updated successfully.");
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("upload-image")]
        public async Task<JsonResult> UploadImage(HttpPostedFileBase file, int? itemId)
        {
            if (itemId == null)
            {
                return Json(new {itemId = 0, url = "" });
            }

            try
            {
                var banner = await _bannerService.GetById(itemId.Value);
                if (banner == null)
                {
                    throw new Exception("Could not upload image. banner not available.");
                }
                var orginalImageUri = await _storageService.SaveFile("banners", file);
               banner.OrginalImgUrl = orginalImageUri.AbsoluteUri;
                var widths = new[] { ImageWidthConstants.BannerImageSm, ImageWidthConstants.BannerImageMd, ImageWidthConstants.BannerImageLg };
                var uris = await UploadImage(file, widths);

                banner.ImgSmUrl = UrlFormatter.GetCdnUrl(uris[0]);
                banner.ImgMdUrl = UrlFormatter.GetCdnUrl(uris[1]);
                banner.ImgLgUrl = UrlFormatter.GetCdnUrl(uris[2]);

                await _bannerService.Update(banner);

                return Json(new
                {
                    itemId = banner.BannerId,
                    url = banner.ImgSmUrl
                });
            }
            catch (Exception)
            {
                return JsonError("Could not upload image.");
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("delete-image")]
        public async Task<JsonResult> DeleteImage(int itemId)
        {
            try
            {
                var banner = await _bannerService.GetById(itemId);
                if (banner != null)
                {
                    var bannerImages = new[] { banner.ImgSmUrl, banner.ImgMdUrl, banner.ImgLgUrl };
                    var fileNames = bannerImages.Select(Path.GetFileName);

                    foreach (var fileName in fileNames.Where(i => !string.IsNullOrWhiteSpace(i)))
                    {
                        await _storageService.DeleteFile("banners", fileName);
                    }
                    banner.ImgSmUrl = "";
                    banner.ImgMdUrl = "";
                    banner.ImgLgUrl = "";

                    await _bannerService.Update(banner);
                }

                return Json(HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return JsonError("Could not delete image.");
            }
        }

        [Route("{bannerId:int}/delete")]
        public async Task<ActionResult> Delete(int bannerId)
        {
            var banner = await _bannerService.GetById(bannerId);
            if (banner == null)
            {
                return HttpNotFound();
            }
            return DeleteConfirmation("Delete banner?", "Are you sure you want to delete this banner?");
        }

        [Route("{bannerId:int}/delete")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int bannerId, FormCollection formCollection)
        {
            var banner = await _bannerService.GetById(bannerId);
            if (banner == null)
            {
                return HttpNotFound();
            }
            await _bannerService.Delete(banner);
            SetStatusMessage("Banner has been deleted successfully.");
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
                ForceSquare = false,
                Widths = widths
            };

            var uris = await _storageService.SaveImage("banners", file, imageSettings);
            if (uris.Length != widths.Length)
            {
                throw new Exception("Could not upload image, image service did not return expected results.");
            }

            return uris;
        }
    }
}