using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Models;

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
                    CurrentSubscription = user.SubscriptionUtc.Value;
                }
            }
            else
            {
                CurrentSubscription=DateTime.Today;
            }
            AddDays = 30;
            
            return this;
        }
    }
}
