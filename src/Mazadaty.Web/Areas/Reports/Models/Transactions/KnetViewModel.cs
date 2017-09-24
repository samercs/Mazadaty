using Mazadaty.Models.Payment;
using Mazadaty.Web.Models;
using System.Collections.Generic;

namespace Mazadaty.Web.Areas.Reports.Models.Transactions
{
    public class KnetViewModel
    {
        public DateRangeModel DateRange { get; set; }

        public IReadOnlyCollection<KnetTransaction> KnetTransactions { get; set; }
    }
}
