using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mzayad.Web.Areas.Admin.Models.Pages;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Base.Core.Formatting;
using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Models;
using Mzayad.Models.Enums;
using Mzayad.Services;

namespace Mzayad.Web.Areas.Admin.Controllers
{
    [RoleAuthorize(Role.Administrator), RouteArea("admin"), RoutePrefix("pages")]
    public class PageManagementController : ApplicationController
    {
        private readonly PageService _pageService;
        public PageManagementController(IAppServices appServices) : base(appServices)
        {
            _pageService = new PageService(DataContextFactory);
        }

        [Route("")]
        public async Task<ActionResult> Index()
        {
            var pages = await _pageService.GetPages();
            return View(pages);
        }

        [HttpPost]
        [Route("json")]
        public async Task<JsonResult> GetPages([DataSourceRequest] DataSourceRequest request)
        {
            var results = await _pageService.GetPages();
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
                    Title ="",
                    Content = "",
                    Status = PageStatus.Pupblic,
                    UserId = AuthService.CurrentUserId(),
                    Author = NameFormatter.GetFullName(user.FirstName, user.LastName),
                    
                }
            };

            return View(model);
        }

        [Route("add")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddEditViewModel model)
        {
            var user = await AuthService.CurrentUser();

            model.Page.UserId = user.Id;
            model.Page.Author = NameFormatter.GetFullName(user.FirstName, user.LastName);
            model.Page.Status = PageStatus.Pupblic;

            //if (!ModelState.IsValid)
            //{
            //    return View(new AddEditViewModel { Page = page });
            //}
            model.Page.Content = $"--{model.Page.Title}--";
            var page = await _pageService.AddPage(model.Page);
            return RedirectToAction("Edit", new { id = page.PageId });
        }

        [Route("edit/{id}")]
        public async Task<ActionResult> Edit(int id)
        {
            var page = await _pageService.GetById(id);
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
        public async Task<ActionResult> Edit(int id, AddEditViewModel model)
        {
            var page = await _pageService.GetById(id);
            if (page == null)
            {
                return HttpNotFound();
            }

            
            page.Author = model.Page.Author;
            page.PageTag = model.Page.PageTag;
            page.Status = model.Page.Status;
            page.Content = model.Page.Content;
            page.Status = model.Page.Status;

            await _pageService.Update(page);

            SetStatusMessage("Page has been saved successfully.");
            return RedirectToAction("Index");
        }

        [Route("delete/{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var page = await _pageService.GetById(id);
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
            var page = await _pageService.GetById(id);
            if (page == null)
            {
                return HttpNotFound();
            }
            await _pageService.Delete(page);

            SetStatusMessage("Page successfully deleted.");
            return RedirectToAction("Index");
        }

    }
}