using Mzayad.Core.Formatting;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Services.Client.Storage;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;

namespace Mzayad.Web.Areas.admin.Controllers
{
    [RoleAuthorize(Role.Administrator)]
    public class AvatarsController : ApplicationController
    {
        private readonly AvatarService _avatarService;
        private readonly IStorageService _storageService;

        public AvatarsController(IAppServices appServices, IStorageService storageService)
            : base(appServices)
        {
            _avatarService = new AvatarService(appServices.DataContextFactory);
            _storageService = storageService;
        }

        public async Task<ActionResult> Index()
        {
            var model = await _avatarService.GetAll();
            return View(model);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(HttpPostedFileBase upload)
        {
            var url = await UploadImage(upload);
            var avatar = new Avatar
            {
                Url = UrlFormatter.GetCdnUrl(url.Single())
            };

            await _avatarService.Add(avatar);

            SetStatusMessage("Avatar successfully uploaded.");
            return RedirectToAction("Index");
        }

        public ActionResult Delete()
        {
            return DeleteConfirmation("Delete Avatar", "Are you sure you want permanently delete this avatar?");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var avatar = await _avatarService.GetById(id);
            if (avatar == null)
            {
                return HttpNotFound();
            }

            await _avatarService.Delete(avatar);

            SetStatusMessage("Avatar successfully deleted.");
            return RedirectToAction("Index");
        }

        private async Task<Uri[]> UploadImage(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0 || !file.ContentType.Contains("image"))
            {
                throw new ArgumentException("Please only upload an image.");
            }

            var imageSettings = new ImageSettings
            {
                Widths = new[] { 512 },
                BackgroundColor = Color.White,
                ForceSquare = true,
            };

            var uris = await _storageService.SaveImage("avatars", file, imageSettings);
            if (!uris.Any())
            {
                throw new Exception("Could not upload image, image service did not return expected results.");
            }

            return uris;
        }

        public async Task<ActionResult> UpdateOrder(int oldIndex, int newIndex)
        {
            var allAvatar = await _avatarService.GetAll();
            var avatar = allAvatar.ElementAt(oldIndex);
            if (allAvatar.Count() > 1 && newIndex < allAvatar.Count())
            {
                double index = 0;
                //move to first image
                if (newIndex == 0)
                {
                    index = allAvatar.ElementAt(0).SortOrder / 2;
                }
                //move to last image
                else if ((newIndex + 1) == allAvatar.Count())
                {
                    index = allAvatar.Last().SortOrder + 1;
                }
                //move between two image
                else
                {
                    if (oldIndex < newIndex)
                    {
                        index = (allAvatar.ElementAt(newIndex).SortOrder + allAvatar.ElementAt(newIndex + 1).SortOrder) / 2;
                    }
                    else
                    {
                        index = (allAvatar.ElementAt(newIndex).SortOrder + allAvatar.ElementAt(newIndex - 1).SortOrder) / 2;
                    }

                }

                avatar.SortOrder = index;
                await _avatarService.Update(avatar);
            }

            return Content("done");
        }
    }
}