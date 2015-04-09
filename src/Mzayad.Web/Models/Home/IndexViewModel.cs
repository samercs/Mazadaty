using System.Collections.Generic;
using System.Linq;
using Mzayad.Models;

namespace Mzayad.Web.Models.Home
{
    public class IndexViewModel
    {
        public IEnumerable<Auction> Auctions { get; set; }

        public IEnumerable<Auction> LiveAuctions
        {
            get { return Auctions.Where(i => i.IsLive()); }
        } 
    }
}
