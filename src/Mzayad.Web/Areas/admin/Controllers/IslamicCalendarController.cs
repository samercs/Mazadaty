using System.Threading.Tasks;
using System.Web.Mvc;
using Mzayad.Services;
using Mzayad.Web.Areas.Admin.Models.IslamicCalendars;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Attributes;
using Mzayad.Web.Core.Identity;
using Mzayad.Web.Core.Services;
using Mzayad.Models;

namespace Mzayad.Web.Areas.Admin.Controllers
{
    [RouteArea("admin"), RoutePrefix("islamiccalendars"), RoleAuthorize(Role.Administrator)]
    public class IslamicCalendarController : ApplicationController
    {
        private readonly IslamicCalendarService _islamicCalendarService;

        public IslamicCalendarController(IAppServices appService) : base(appService)
        {
            _islamicCalendarService = new IslamicCalendarService(DataContextFactory);   
        } 

        public async Task<ActionResult> Index()
        {
            var model = await _islamicCalendarService.GetAll();
            return View(model);
        }

        [Route("edit/{id}")]
        public async Task<ActionResult> Edit(int id)
        {
            var calendar = await _islamicCalendarService.GetById(id);
            if (calendar == null)
            {
                return HttpNotFound();
            }

            //var viewModel = new EditViewModel
            //{
            //    Calendar = calendar
            //};

            return View(calendar);
        }
        
        [Route("edit/{id}")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, IslamicCalendar model)
        {
            var calendar = await _islamicCalendarService.GetById(id);
            if (calendar == null)
            {
                return HttpNotFound();
            }

            calendar.EidAdhaFrom = model.EidAdhaFrom;
            calendar.EidAdhaTo= model.EidAdhaTo;
            calendar.EidFetrFrom = model.EidFetrFrom;
            calendar.EidFetrTo = model.EidFetrTo;
            calendar.HijriYear = model.HijriYear;
            calendar.NewYear = model.NewYear;

            await _islamicCalendarService.Update(calendar);

            SetStatusMessage("Islamic Calendar successfully updated.");

            return RedirectToAction("Index");
        }
    }
}