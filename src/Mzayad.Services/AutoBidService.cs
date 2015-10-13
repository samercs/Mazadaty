using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Mzayad.Data;
using Mzayad.Models;
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

        public ApplicationUser TryGetAutoBid(int auctionId, int secondsLeft)
        {
            if (secondsLeft <= 0)
            {
                return null;
            }

            var factor = GetTimeZoneFactor(secondsLeft);
            var frequency = factor/12;
            if (secondsLeft%frequency != 0)
            {
                return null;
            }

            using (var dc = DataContext())
            {
                return dc.AutoBids
                    .Include(i => i.User)
                    .FirstOrDefault()?.User;
            }
        }

        private static int GetTimeZoneFactor(int secondsLeft)
        {
            if (secondsLeft <= 12)
            {
                return 12;
            }

            var x = 12;

            while (true)
            {
                x = x*2;

                if (x >= secondsLeft)
                {
                    return x;
                }
            }
        }
    }
}
