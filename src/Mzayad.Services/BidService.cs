using Mzayad.Core.Extensions;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Services.Queues;
using OrangeJetpack.Localization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Mzayad.Services
{
    public class BidService : ServiceBase
    {
        private readonly IQueueService _queueService;

        public BidService(IDataContextFactory dataContextFactory, IQueueService queueService) : base(dataContextFactory)
        {
            _queueService = queueService;
        }

        public Bid SubmitUserBid(int auctionId, int secondsLeft, string userId)
        {
            using (var dc = DataContext())
            {
                var bid = dc.SubmitUserBid(auctionId, secondsLeft, userId);

                if (bid != null)
                {
                    _queueService.LogBid(bid);
                }

                return bid;
            }
        }

        /// <summary>
        /// Gets a user with valid autobid configurations for current auction.
        /// </summary>
        public Bid TrySubmitAutoBid(int auctionId, int secondsLeft)
        {
            if (!ShouldAutoBid(secondsLeft))
            {
                return null;
            }

            using (var dc = DataContext())
            {
                var bid = dc.SubmitAutoBid(auctionId, secondsLeft);

                if (bid != null)
                {
                    _queueService.LogBid(bid);
                }

                return bid;
            }
        }

        /// <summary>
        /// Gets whether or not an autobid should be attempted based on the time left in an auction.
        /// /// </summary>
        private static bool ShouldAutoBid(int secondsLeft)
        {
            var percentage = 100d / secondsLeft;
            var random = new Random();
            var probability = random.Next(1000) / 10d;
            var shouldBid = probability <= percentage;

            Trace.TraceInformation($"ShouldAutoBid: secondsLeft: {secondsLeft}, percentage: {percentage}, probability: {probability}, shouldBid: {shouldBid}");

            return shouldBid;
        }

        /// <summary>
        /// Gets an auction's bid with the highest amount.
        /// </summary>
        public async Task<Bid> GetWinningBid(int auctionId)
        {
            using (var dc = DataContext())
            {
                return await dc.Bids
                    .Include(i => i.User.Address)
                    .Where(i => i.AuctionId == auctionId)
                    .OrderByDescending(i => i.Amount)
                    .ThenByDescending(i => i.CreatedUtc)
                    .FirstOrDefaultAsync();
            }
        }

        public async Task<IReadOnlyCollection<Bid>> GetRecentBidHistoryForUser(string userId, string language)
        {
            using (var dc = DataContext())
            {
                var bids = await dc.Bids
                    .Include(i => i.Auction)
                    .Where(i => i.UserId == userId)
                    .OrderByDescending(i => i.CreatedUtc)
                    .Take(20)
                    .ToListAsync();

                foreach (var auction in bids.Select(i => i.Auction).Distinct())
                {
                    auction.Localize(language, i => i.Title);
                }

                return bids;
            }
        }

        public async Task<IReadOnlyCollection<Bid>> GetBidHistoryForAuction(int auctionId)
        {
            using (var dc = DataContext())
            {
                return await dc.Bids
                    .Include(i => i.User)
                    .Where(i => i.AuctionId == auctionId)
                    .OrderByDescending(i => i.BidId)
                    .ToListAsync();
            }
        }

        public async Task<IReadOnlyCollection<Bid>> GetBidHistoryForUser(string userId, string language)
        {
            using (var dc = DataContext())
            {
                var bids = await dc.Bids
                    .Include(i => i.Auction.Product.ProductImages)
                    .Include(i => i.Auction.WonByUser)
                    .Include(i => i.Auction.Bids)
                    .Where(i => i.UserId == userId)
                    .OrderByDescending(i => i.CreatedUtc)
                    .ToListAsync();

                foreach (var auction in bids.Select(i => i.Auction).Distinct())
                {
                    auction.Localize(language, i => i.Title);
                }

                return bids;
            }
        }

        public class AuctionBidHistory : ILocalizable
        {
            public int AuctionId { get; set; }

            [Localized]
            public string Title { get; set; }
            public ProductImage ProductImage { get; set; }
            public string ProductImageUrl => ProductImage != null ? ProductImage.ImageMdUrl : ProductImage.NoImageUrl;
            public DateTime StartUtc { get; set; }
            public DateTime? ClosedUtc { get; set; }
            public decimal? WonAmount { get; set; }
            public ApplicationUser WonUser { get; set; }
            public int UserBidsCount { get; set; }
            public decimal MaximumBid { get; set; }
        }

        public async Task<IReadOnlyCollection<AuctionBidHistory>> GetAuctionBidHistoryForUser(string userId, string language)
        {
            using (var dc = DataContext())
            {
                ((DbContext)dc).Database.Log = s => Trace.TraceInformation(s);

                var auctions = await dc.Auctions
                    .Include(i => i.Product.ProductImages)
                    .Where(i => i.Bids.Any(j => j.UserId == userId))
                    .OrderByDescending(i => i.AuctionId)
                    .Select(i => new AuctionBidHistory
                    {
                        AuctionId = i.AuctionId,
                        Title = i.Title,
                        ProductImage = i.Product.ProductImages.FirstOrDefault(),
                        StartUtc = i.StartUtc,
                        ClosedUtc = i.ClosedUtc,
                        WonAmount = i.WonAmount,
                        WonUser = i.WonByUser,
                        UserBidsCount = i.Bids.Count(j => j.UserId == userId),
                        MaximumBid = i.Bids.Where(j => j.UserId == userId).Max(h => h.Amount)
                    })
                    .ToListAsync();

                foreach (var auction in auctions.Distinct())
                {
                    auction.Localize(language, i => i.Title);
                }

                return auctions;
            }
        }

        public async Task<IReadOnlyCollection<Bid>> GetByUser(string userId, DateTime? from = null)
        {
            using (var dc = DataContext())
            {
                return await dc.Bids
                    .Where(i => i.UserId == userId && (from.HasValue && i.CreatedUtc.Date >= from.Value.Date || !from.HasValue))
                    .GroupBy(i => i.CreatedUtc.Date)
                    .Select(g => g.OrderByDescending(i => i.CreatedUtc).First())
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Gets the total amount of consecutive days that a user has bid since today.
        /// </summary>
        public async Task<int> GetConsecutiveBidDays(string userId)
        {
            using (var dc = DataContext())
            {
                var userBidDates = await dc.Bids
                    .Where(i => i.UserId == userId)
                    .OrderByDescending(i => i.CreatedUtc)
                    .Select(i => i.CreatedUtc)
                    .ToListAsync();

                return userBidDates.Consecutive();
            }
        }

        public async Task<int> CountUserBids(string userId, DateTime? from = null)
        {
            using (var db = DataContext())
            {
                return await db.Bids.CountAsync(i => i.UserId == userId && (
                                                                (from.HasValue && i.CreatedUtc >= from.Value)
                                                                ||
                                                                !from.HasValue));
            }
        }
    }
}
