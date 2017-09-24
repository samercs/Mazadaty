using Mazadaty.Models;
using Mazadaty.Web.Models;
using System.Collections.Generic;

namespace Mazadaty.Web.Areas.Reports.Models.Products
{
    public class IndexViewModel
    {
        public DateRangeModel DateRange { get; set; }

        public IReadOnlyCollection<Product> Products { get; set; }
    }
}
