using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Mzayad.Services
{
    public class AutoBidService : ServiceBase
    {
        public AutoBidService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<AutoBid> Get(string userId, int auctionId)
        {
            using (var dc = DataContext())
            {
                var autoBid = await dc.AutoBids.FirstOrDefaultAsync(i => i.UserId == userId && i.AuctionId == auctionId);

                return autoBid;
            }
        }

        public async Task<IReadOnlyCollection<AutoBid>> GetAutoBiddersForNotification()
        {
            using (var dc = DataContext())
            {
                return await dc.AutoBids
                    .Include(i => i.Auction)
                    .Include(i => i.User)
                    .Where(i => i.Auction.StartUtc >= DbFunctions.AddMinutes(DateTime.UtcNow, 15))
                    .Where(i => i.Auction.StartUtc <= DbFunctions.AddMinutes(DateTime.UtcNow, 75))
                    .Where(i => i.Auction.Status == AuctionStatus.Public)
                    .Where(i => i.User.AutoBidNotification)
                    .ToListAsync();
            }
        }

        public async Task<IReadOnlyCollection<AutoBid>> GetAutoBidsForAuction(int auctionId)
        {
            using (var dc = DataContext())
            {
                return await dc.AutoBids
                    .Include(i => i.User)
                    .Where(i => i.AuctionId == auctionId)
                    .OrderByDescending(i => i.MaxBid)
                    .ToListAsync();
            }
        }

        public async Task Save(string userId, int auctionId, decimal maxBid)
        {
            using (var dc = DataContext())
            {
                await Delete(userId, auctionId);

                dc.AutoBids.Add(new AutoBid
                {
                    UserId = userId,
                    AuctionId = auctionId,
                    MaxBid = maxBid
                });

                await dc.SaveChangesAsync();
            }
        }

        public async Task Delete(string userId, int auctionId)
        {
            using (var dc = DataContext())
            {
                var autoBids =
                    await dc.AutoBids.Where(i => i.UserId == userId && i.AuctionId == auctionId).ToListAsync();

                foreach (var autoBid in autoBids)
                {
                    dc.AutoBids.Remove(autoBid);
                }

                await dc.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets whether or not an autobid should be attempted based on the time left in an auction.
        /// /// </summary>
        public bool ShouldAutoBid(int secondsLeft)
        {
            var autoBidSeconds = new[] { 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };
            
            return autoBidSeconds.Contains(secondsLeft);
        }

        /// <summary>
        /// Gets a user with valid autobid configurations for current auction.
        /// </summary>
        public string TryGetAutoBid(int auctionId, string lastBidUserId, decimal lastBidAmount)
        {
            using (var dc = DataContext())
            {
                return dc.AutoBids
                    .Where(i => i.AuctionId == auctionId)
                    .Where(i => i.UserId != lastBidUserId)
                    .Where(i => i.MaxBid > lastBidAmount)
                    .OrderBy(c => Guid.NewGuid())
                    .Select(i => i.UserId)
                    .FirstOrDefault();
            }
        }

        private static bool FallsOnAutoBidSecond(int secondsLeft)
        {
            var factor = GetTimeZoneFactor(secondsLeft);
            var frequency = factor / 12;

            return secondsLeft % frequency == 0;
        }

        private static int GetTimeZoneFactor(int secondsLeft)
        {
            var x = 12;

            while (true)
            {
                if (x >= secondsLeft)
                {
                    return x;
                }

                x = x * 2;
            }
        }

        public async Task<int> CountUserAutoBids(string userId, DateTime? from = null)
        {
            using (var db = DataContext())
            {
                return await db.AutoBids.Where(i => i.UserId == userId && (
                                                                (from.HasValue && i.CreatedUtc >= from.Value)
                                                                ||
                                                                !from.HasValue))
                                        .Select(i => i.AuctionId)
                                        .Distinct()
                                        .CountAsync();
            }
        }
    }
}
