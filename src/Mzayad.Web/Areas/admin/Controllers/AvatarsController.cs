using Mzayad.Core.Formatting;
using Mzayad.Models;
using Mzayad.Services;
using Mzayad.Web.Areas.admin.Models.Avatar;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Services.Client.Storage;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mzayad.Web.Areas.admin.Controllers
{
    [RoleAuthorize(Role.Administrator)]
    [RouteArea("admin"), RoutePrefix("avatars")]
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
            var model = new AvatarAddViewModel
            {
                Avatar = new Avatar()
            };
            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(HttpPostedFileBase upload, AvatarAddViewModel model)
        {
            var url = await UploadImage(upload);
            var avatar = new Avatar
            {
                Url = UrlFormatter.GetCdnUrl(url.Single()),
                Token = model.Avatar.IsPremium ? model.Avatar.Token : null,
                IsPremium = model.Avatar.IsPremium
            };

            await _avatarService.Add(avatar);

            SetStatusMessage("Avatar successfully uploaded.");
            return RedirectToAction("Index");
        }

        [Route("edit/{id:int}")]
        public async Task<ActionResult> Edit(int id)
        {
            var avatar = await _avatarService.GetById(id);
            if (avatar == null)
            {
                return HttpNotFound();
            }
            var model = new AvatarAddViewModel
            {
                Avatar = avatar
            };
            return View(model);
        }

        [Route("edit/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, AvatarAddViewModel model)
        {
            var avatar = await _avatarService.GetById(id);
            if (avatar == null)
            {
                return HttpNotFound();
            }
            avatar.IsPremium = model.Avatar.IsPremium;
            avatar.Token = model.Avatar.IsPremium ? model.Avatar.Token : null;
            await _avatarService.Save(avatar);
            SetStatusMessage("Avatar information has been updated successfully.");
            return RedirectToAction("Index");
        }

        [Route("delete/{id:int}")]
        public ActionResult Delete(int id)
        {
            return DeleteConfirmation("Delete Avatar", "Are you sure you want permanently delete this avatar?");
        }

        [Route("delete/{id:int}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, FormCollection formCollection)
        {
            var avatar = await _avatarService.GetById(id);
            if (avatar == null)
            {
                return HttpNotFound();
            }

            await StorageService.DeleteFile("avatars", Path.GetFileName(avatar.Url));
            await _avatarService.Delete(avatar);

            SetStatusMessage("Avatar successfully deleted.");
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> UploadAvatarImage(HttpPostedFileBase file, int itemId)
        {
            //await DeleteImage(itemId.Value);
            try
            {
                var avatar = await _avatarService.GetById(itemId);
                if (avatar == null)
                {
                    throw new Exception("Could not upload image. Avatar not available.");
                }

                var newUrl = await UploadImage(file);
                avatar.Url = UrlFormatter.GetCdnUrl(newUrl.Single());
                await _avatarService.Save(avatar);
                return Json(new
                {
                    itemId = avatar.AvatarId,
                    url = avatar.Url
                });
            }
            catch (Exception)
            {
                return JsonError("Could not upload image.");
            }
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