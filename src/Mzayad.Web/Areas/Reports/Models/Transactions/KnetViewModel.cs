using Mzayad.Models.Payment;
using Mzayad.Web.Models;
using System.Collections.Generic;

namespace Mzayad.Web.Areas.Reports.Models.Transactions
{
    public class KnetViewModel
    {
        public DateRangeModel DateRange { get; set; }

        public IReadOnlyCollection<KnetTransaction> KnetTransactions { get; set; }
    }
}