using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mzayad.Web.Areas.Api.Models.Orders
{
    public class GetShippingViewModel
    {
        public IList<int> ProductIds { get; set; }
    }
}