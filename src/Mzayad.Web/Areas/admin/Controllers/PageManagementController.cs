using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mzayad.Web.Areas.Admin.Models.Pages;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Base.Core.Formatting;
using OrangeJetpack.Cms.Models;
using OrangeJetpack.Localization;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Mzayad.Web.Areas.Admin.Controllers
{
    [RoleAuthorize(Role.Administrator), RouteArea("admin"), RoutePrefix("pages")]
    public class PageManagementController : ApplicationController
    {
        public PageManagementController(IAppServices appServices) : base(appServices)
        {

        }

        [Route("")]
        public async Task<ActionResult> Index()
        {
            var pages = await CmsClient.GetPages(Language);
            return View(pages);
        }

        [HttpPost]
        [Route("json")]
        public async Task<JsonResult> GetPages([DataSourceRequest] DataSourceRequest request)
        {
            var results = await CmsClient.GetPages();
            return Json(results.ToDataSourceResult(request));
        }

        [Route("add")]
        public async Task<ActionResult> Add()
        {
            var user = await AuthService.CurrentUser();

            var model = new AddEditViewModel
            {
                Page = new Page
                {
                    Title = LocalizedContent.Init(),
                    Html = LocalizedContent.Init(),
                    Status = ItemStatus.Private,
                    UserId = AuthService.CurrentUserId(),
                    Author = NameFormatter.GetFullName(user.FirstName, user.LastName),
                    //ProjectKey = AppSettings.ProjectKey
                },
            };

            return View(model);
        }

        [Route("add")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(Page page, LocalizedContent[] title)
        {
            var user = await AuthService.CurrentUser();

            page.Title = title.Serialize();
            page.UserId = user.Id;
            page.Author = NameFormatter.GetFullName(user.FirstName, user.LastName);
            //page.ProjectKey = AppSettings.ProjectKey;

            if (!ModelState.IsValid)
            {
                return View(new AddEditViewModel { Page = page });
            }

            page = await CmsClient.AddPage(page);
            return RedirectToAction("Edit", new { id = page.PageId });
        }

        [Route("edit/{id}")]
        public async Task<ActionResult> Edit(int id)
        {
            var page = await CmsClient.GetPageById(id);
            if (page == null)
            {
                return HttpNotFound();
            }

            var model = new AddEditViewModel
            {
                Page = page
            };

            return View(model);
        }

        [Route("edit/{id}")]
        [HttpPost, ValidateAntiForgeryToken, ValidateInput(false)]
        public async Task<ActionResult> Edit(int id, AddEditViewModel model, LocalizedContent[] title, LocalizedContent[] html)
        {
            var page = await CmsClient.GetPageById(id);
            if (page == null)
            {
                return HttpNotFound();
            }

            page.Title = title.Serialize();
            page.Author = model.Page.Author;
            page.Slug = model.Page.Slug;
            page.Status = model.Page.Status;
            page.Html = html.Serialize();

            await CmsClient.UpdatePage(id, page);

            SetStatusMessage("Page has been saved successfully.");
            return RedirectToAction("Index", new { folderId = page.FolderId });
        }

        [Route("delete/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var page = await CmsClient.GetPageById(id);
            if (page == null)
            {
                return HttpNotFound();
            }

            return DeleteConfirmation("Delete Page", "Are you sure you want to permanently delete this page?");
        }

        [HttpPost]
        [Route("delete/{id:int}")]
        public async Task<ActionResult> DeletePage(int id)
        {
            var page = await CmsClient.GetPageById(id);
            if (page == null)
            {
                return HttpNotFound();
            }
            await CmsClient.DeletePage(id);

            SetStatusMessage("Page successfully deleted.");
            return RedirectToAction("Index", new { folderId = page.FolderId });
        }

    }
}