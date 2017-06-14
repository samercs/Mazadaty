using Mzayad.Models;
using System.Collections.Generic;

namespace Mzayad.Web.Areas.Admin.Models.Home
{
    public class BuyNowModel
    {
        public string Search { get; set; }
        public IReadOnlyCollection<Auction> Auctions { get; set; }
    }
}
