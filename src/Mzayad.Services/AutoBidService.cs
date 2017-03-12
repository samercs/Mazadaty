using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
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
