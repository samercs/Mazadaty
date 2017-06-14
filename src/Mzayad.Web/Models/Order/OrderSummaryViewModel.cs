using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Glimpse.Core.Extensions;
using Mzayad.Models.Enum;
using Mzayad.Web.Extensions;

namespace Mzayad.Web.Models.Order
{
    public class OrderSummaryViewModel
    {
        public Mzayad.Models.Order Order { get; set; }
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
