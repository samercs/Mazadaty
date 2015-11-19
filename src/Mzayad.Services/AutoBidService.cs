﻿using System;
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

        public ApplicationUser TryGetAutoBid(int auctionId, int secondsLeft, decimal? lastBidAmount)
        {
            if (secondsLeft <= 0 || !FallsOnAutoBidSecond(secondsLeft))
            {
                return null;
            }

            lastBidAmount = lastBidAmount ?? 0;

            using (var dc = DataContext())
            {
                var autoBids = dc.AutoBids
                    .Where(i => i.AuctionId == auctionId)
                    .Where(i => i.MaxBid > lastBidAmount)
                    .OrderBy(c => Guid.NewGuid())
                    .FirstOrDefault();

                return autoBids?.User;
            }
        }

        private static bool FallsOnAutoBidSecond(int secondsLeft)
        {
            var factor = GetTimeZoneFactor(secondsLeft);
            var frequency = factor / 12;

            return secondsLeft%frequency == 0;
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
    }
}