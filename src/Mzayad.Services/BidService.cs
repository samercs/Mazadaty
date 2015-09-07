using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mzayad.Data;
using Mzayad.Models;
using OrangeJetpack.Localization;
using System;

namespace Mzayad.Services
{
    public class BidService : ServiceBase
    {
        public BidService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task AddBid(int auctionId, string userId, decimal amount, int secondsLeft, string hostAddress)
        {
            using (var dc = DataContext())
            {
                dc.Bids.Add(new Bid
                {
                    AuctionId = auctionId,
                    UserId = userId,
                    Amount = amount,
                    SecondsLeft = secondsLeft,
                    UserHostAddress = hostAddress
                });

                await dc.SaveChangesAsync();
            }
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

        public async Task<IReadOnlyCollection<Bid>> GetBidHistoryForUser(string userId, string language)
        {
            using (var dc = DataContext())
            {
                var bids = await dc.Bids
                    .Include(i => i.Auction)
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

        public async Task<IReadOnlyCollection<Bid>> GetByUser(string userId, DateTime? from = null)
        {
            using (var dc = DataContext())
            {
                return await dc.Bids
                    .Where(i => i.UserId == userId && (from.HasValue && i.CreatedUtc.Date >= from.Value.Date || !from.HasValue))
                    .GroupBy(i => i.CreatedUtc.Date)
                    .Select(g => g.OrderByDescending(i=> i.CreatedUtc).First())
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

                // if user has never bid or has not bid today
                if (!userBidDates.Any() || userBidDates.First().Date != DateTime.UtcNow.Date)
                {
                    return 0;
                }

                var bidsGroupedByDate = userBidDates.GroupBy(i => i.Date).ToList();

                var streak = 1;

                for (var i = 0; i < bidsGroupedByDate.Count - 1; i++)
                {
                    var dateDifference = bidsGroupedByDate[i].Key.Subtract(bidsGroupedByDate[i+1].Key).Days;
                    if (dateDifference != 1)
                    {
                        return streak;
                    }

                    streak++;
                }

                return streak;
            }
        }
    }
}
