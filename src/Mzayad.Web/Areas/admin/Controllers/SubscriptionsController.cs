using Mzayad.Services;
using Mzayad.Web.Controllers;
using Mzayad.Web.Core.Services;
using OrangeJetpack.Localization;
using System.Threading.Tasks;
using System.Web.Mvc;
using AddViewModel = Mzayad.Web.Areas.admin.Models.Subscription.AddViewModel;

namespace Mzayad.Web.Areas.admin.Controllers
{
    public class SubscriptionsController : ApplicationController
    {
        private readonly SubscriptionService _subscriptionService;

        public SubscriptionsController(IAppServices appServices)
            : base(appServices)
        {
            _subscriptionService = new SubscriptionService(DataContextFactory);
        }

        public async Task<ActionResult> Index()
        {
            var model = await _subscriptionService.GetAll("en");
            return View(model);
        }

        public ActionResult Add()
        {
            var model = new AddViewModel().Hydrate();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AddViewModel model, LocalizedContent[] name)
        {

            model.Subscription.Name = name.Serialize();
            await _subscriptionService.Add(model.Subscription);
            SetStatusMessage("Subscription has been added successfully.");
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> Edit(int id)
        {
            var subscription = await _subscriptionService.GetById(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }

            var model = new AddViewModel()
            {
                Subscription = subscription
            };

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, AddViewModel model, LocalizedContent[] name)
        {
            var subscription = await _subscriptionService.GetById(id);
            if (subscription == null)
            {
                return HttpNotFound();
            }

            subscription.Name = name.Serialize();
            subscription.Status = model.Subscription.Status;
            subscription.Quantity = model.Subscription.Quantity;
            subscription.ExpirationUtc = model.Subscription.ExpirationUtc;

            await _subscriptionService.Edit(subscription);
            SetStatusMessage("Subscription has been updated successfully.");
            return RedirectToAction("Index");
        }
    }
}