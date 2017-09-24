using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Glimpse.Core.Extensions;
using Mazadaty.Models.Enum;
using Mazadaty.Web.Extensions;

namespace Mazadaty.Web.Models.Order
{
    public class OrderSummaryViewModel
    {
        public Mazadaty.Models.Order Order { get; set; }
        public string Language { get; set; }

        public IEnumerable<SelectListItem> PaymentMethod
        {
            get
            {
                return Enum.GetValues(typeof (PaymentMethod)).Cast<PaymentMethod>().Select(i => new SelectListItem()
                {
                    Text = i.ToString(),
                    Value = i.ToString(),
                    Selected = i==Order.PaymentMethod
                }
                    );
            }
        }

        

    }
}
