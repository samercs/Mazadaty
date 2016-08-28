using Mzayad.Models;
using System.Collections.Generic;
using System.Linq;
using Mzayad.Web.Core.Enums;
using Mzayad.Web.Models.Auctions;
using WebGrease.Css.Extensions;

namespace Mzayad.Web.Models.Home
{
    public class IndexViewModel
    {
        public IReadOnlyCollection<AuctionViewModel> ClosedAuctions { get; set; }
        public IReadOnlyCollection<AuctionViewModel> UpcomingAuctions { get; set; }
        public IReadOnlyCollection<AuctionViewModel> LiveAuctions { get; set; }
        public IReadOnlyCollection<int> LiveAuctionIds { get; set; }

        public IndexViewModel(IEnumerable<Auction> auctions, IEnumerable<Auction> closedAuctions, IEnumerable<Auction> upcomingAuctions)
        {
            ClosedAuctions = closedAuctions.Select(AuctionViewModel.Create).ToList();
            ClosedAuctions.ForEach(i => i.Type = AuctionType.Closedauction);

            UpcomingAuctions = upcomingAuctions.Select(AuctionViewModel.Create).ToList();
            UpcomingAuctions.ForEach(i => i.Type = AuctionType.UpcomingAuction);

            LiveAuctions = auctions.Where(i => i.IsLive())
                .Select(AuctionViewModel.Create)
                .ToList();

            LiveAuctionIds = LiveAuctions.Select(i => i.AuctionId).ToList();
        }
    }
}
