using Mzayad.Models;
using Mzayad.Web.Models;
using System.Collections.Generic;

namespace Mzayad.Web.Areas.Reports.Models.Products
{
    public class IndexViewModel
    {
        public DateRangeModel DateRange { get; set; }

        public IReadOnlyCollection<Product> Products { get; set; }
    }
}