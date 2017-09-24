using Humanizer;
using Mazadaty.Models.Enum;
using OrangeJetpack.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Mazadaty.Web.Areas.admin.Models.Subscription
{
    public class AddViewModel
    {
        public Mazadaty.Models.Subscription Subscription { get; set; }

        public IEnumerable<SelectListItem> StatusList
        {
            get
            {
                return
                    Enum.GetValues(typeof(SubscriptionStatus))
                        .Cast<SubscriptionStatus>()
                        .Select(i => new SelectListItem
                        {
                            Text = i.Humanize(),
                            Value = i.ToString(),
                            Selected = i == Subscription.Status
                        });
            }
        }
        public AddViewModel Hydrate()
        {
            if (Subscription == null)
            {
                Subscription = new Mazadaty.Models.Subscription
                {
                    Name = LocalizedContent.Init(),
                    Status = SubscriptionStatus.Active,
                    Duration = 30
                };
            }

            return this;
        }
    }
}
