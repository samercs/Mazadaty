using Humanizer;
using Mazadaty.Models;
using Mazadaty.Models.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Mazadaty.Web.Areas.admin.Models.Orders
{
    public class IndexViewModel
    {
        public IEnumerable<OrderModel> Orders { get; set; }
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
