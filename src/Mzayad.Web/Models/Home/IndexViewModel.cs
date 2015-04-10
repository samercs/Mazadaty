using System.Collections.Generic;
using System.Linq;
using Mzayad.Models;
using Mzayad.Models.Enum;

namespace Mzayad.Web.Models.Home
{
    public class IndexViewModel
    {
        public IEnumerable<AuctionViewModel> Auctions { get; set; }
        public IEnumerable<int> LiveAuctionIds { get; set; } 

        public IndexViewModel(IEnumerable<Auction> auctions)
        {
            Auctions = auctions.Select(AuctionViewModel.Create);
            LiveAuctionIds = auctions.Where(i => i.IsLive()).Select(i => i.AuctionId);
        }
    }

    public class AuctionViewModel
    {
        public int AuctionId { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public IEnumerable<AuctionImageViewModel> Images { get; set; } 

        public static AuctionViewModel Create(Auction auction)
        {
            var viewModel = new AuctionViewModel
            {
                AuctionId = auction.AuctionId,
                Title = auction.Title, 
                Status = GetStatus(auction), 
                Images = GetImages(auction)
            };

            return viewModel;
        }

        private static string GetStatus(Auction auction)
        {
            if (auction.Status == AuctionStatus.Closed)
            {
                return RenderStatus.Closed.ToString();
            }
            else if (auction.IsLive())
            {
                return RenderStatus.Live.ToString();
            }
            else
            {
                return RenderStatus.Upcoming.ToString();
            }
        }

        private static IEnumerable<AuctionImageViewModel> GetImages(Auction auction)
        {
            if (!auction.Product.ProductImages.Any())
            {
                return new[]
                {
                    new AuctionImageViewModel
                    {
                        ImageSmUrl = ProductImage.NoImageUrl,
                        ImageMdUrl = ProductImage.NoImageUrl,
                        ImageLgUrl = ProductImage.NoImageUrl
                    }
                };
            }

            return auction.Product.ProductImages.Select(i => new AuctionImageViewModel
            {
                ImageSmUrl = i.ImageSmUrl,
                ImageMdUrl = i.ImageMdUrl,
                ImageLgUrl = i.ImageLgUrl
            });
        }

        private enum RenderStatus { Live, Closed, Upcoming }
    }

    public class AuctionImageViewModel
    {
        public string ImageSmUrl { get; set; }
        public string ImageMdUrl { get; set; }
        public string ImageLgUrl { get; set; }
    }
}
