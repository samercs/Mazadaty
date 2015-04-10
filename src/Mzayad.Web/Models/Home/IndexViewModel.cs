using System.Collections.Generic;
using System.Linq;
using Mzayad.Models;

namespace Mzayad.Web.Models.Home
{
    public class IndexViewModel
    {
        public IEnumerable<AuctionViewModel> Auctions { get; set; }

        //public IEnumerable<Auction> LiveAuctions
        //{
        //    get { return Auctions.Where(i => i.IsLive()); }
        //} 
    }

    public class AuctionViewModel
    {
        public int AuctionId { get; set; }
        public string Title { get; set; }

        public static AuctionViewModel Create(Auction auction)
        {
            return new AuctionViewModel
            {
                AuctionId = auction.AuctionId,
                Title = auction.Title
            };
        }
    }
}
