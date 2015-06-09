using Mzayad.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Mzayad.Web.Areas.Admin.Models.Users
{
    public class EditSubscriptionViewModel
    {
        public ApplicationUser User { get; set; }
        public int AddDays { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CurrentSubscription { get; set; }

        public EditSubscriptionViewModel Hydrate(ApplicationUser user)
        {
            User = user;
            if (user.SubscriptionUtc.HasValue)
            {
                if (user.SubscriptionUtc <= DateTime.UtcNow)
                {
                    CurrentSubscription = DateTime.Today;
                }
                else
                {
                    CurrentSubscription = user.SubscriptionUtc.Value.AddHours(3); // UTC -> AST
                }
            }
            else
            {
                CurrentSubscription = DateTime.Today;
            }
            AddDays = 30;

            return this;
        }
    }
}
