using Mazadaty.Models;
using Mazadaty.Web.Models;
using System.Collections.Generic;

namespace Mazadaty.Web.Areas.Reports.Models.Transactions
{
    public class BuyNowViewModel
    {
        public DateRangeModel DateRange { get; set; }

        public IReadOnlyCollection<BuyNowTransactionModel> BuyNowTransactions { get; set; }
    }
}
