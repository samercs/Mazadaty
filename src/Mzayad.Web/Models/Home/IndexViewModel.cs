using Mzayad.Models;
using System.Collections.Generic;
using System.Linq;
using Mzayad.Web.Models.Auctions;

namespace Mzayad.Web.Models.Home
{
    public class IndexViewModel
    {
        public IReadOnlyCollection<AuctionViewModel> ClosedAuctions { get; set; }
        public IReadOnlyCollection<AuctionViewModel> LiveAuctions { get; set; }
        public IReadOnlyCollection<int> LiveAuctionIds { get; set; } 

        public IndexViewModel(IEnumerable<Auction> auctions, IEnumerable<Auction> closedAuctions)
        {
            ClosedAuctions = closedAuctions.Select(AuctionViewModel.Create).ToList();
            
            LiveAuctions = auctions.Where(i => i.IsLive())
                .Select(AuctionViewModel.Create)
                .ToList();
            
            LiveAuctionIds = LiveAuctions.Select(i => i.AuctionId).ToList();
        }
    }
}
