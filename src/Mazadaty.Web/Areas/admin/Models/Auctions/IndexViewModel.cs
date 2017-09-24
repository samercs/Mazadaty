using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mazadaty.Web.Areas.admin.Models.Auction
{
    public class IndexViewModel
    {
        public IEnumerable<Mazadaty.Models.Auction> Auctions { get; set; }
        public string Search { get; set; }

    }
}
