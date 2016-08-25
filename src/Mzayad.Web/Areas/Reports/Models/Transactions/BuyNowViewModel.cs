using Mzayad.Models;
using Mzayad.Web.Models;
using System.Collections.Generic;

namespace Mzayad.Web.Areas.Reports.Models.Transactions
{
    public class BuyNowViewModel
    {
        public DateRangeModel DateRange { get; set; }

        public IReadOnlyCollection<BuyNowTransactionModel> BuyNowTransactions { get; set; }
    }
}