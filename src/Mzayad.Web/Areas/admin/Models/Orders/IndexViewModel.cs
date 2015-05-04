using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Humanizer;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Web.Extensions;

namespace Mzayad.Web.Areas.admin.Models.Orders
{
    public class IndexViewModel
    {
        public IEnumerable<Order> Orders { get; set; }
        public string Search { get; set; }
        public OrderStatus? Status { get; set; }

        public IEnumerable<SelectListItem> OrderStatusList
        {
            get
            {
                return Enum.GetValues(typeof(OrderStatus)).Cast<OrderStatus>().Select(v => new SelectListItem
                {
                    Text = v.Humanize(),
                    Value = v.ToString(),
                    Selected = v.Equals(Status)
                });
            }
        }
    }
}