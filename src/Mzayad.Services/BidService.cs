﻿using Mzayad.Core.Extensions;
using Mzayad.Data;
using Mzayad.Models;
using OrangeJetpack.Localization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mzayad.Models.Enums;

namespace Mzayad.Services
{
    public class BidService : ServiceBase
    {
        public BidService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public bool AddBid(Bid bid)
        {
            using (var dc = DataContext())
            {
                if (!ValidateBid(dc, bid.AuctionId, bid.UserId).Result)
                {
                    return false;
                }

                dc.Bids.Add(bid);
                dc.SaveChanges();
                return true;
            }
        }

        public async Task<bool> AddBidAsync(Bid bid)
        {
            using (var dc = DataContext())
            {

                if (!await ValidateBid(dc, bid.AuctionId, bid.UserId))
                {
                    return false;
                }

                dc.Bids.Add(bid);
                await dc.SaveChangesAsync();
                return true;
            }
        }

        private static async Task<bool> ValidateBid(IDataContext dc, int auctionId, string userId)
        {
            var lastBid = await dc.Bids
                .Include(i => i.Auction)
                .Where(i => i.AuctionId == auctionId)
                .OrderByDescending(i => i.BidId)
                .FirstOrDefaultAsync();

            if (lastBid == null || lastBid.UserId == userId)
            {
                return false;
            }

            if (lastBid.Auction.Status != AuctionStatus.Public || lastBid.Auction.WonByBidId.HasValue)
            {
                return false;
            }

            return true;
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
                    .OrderByDescending(i => i.CreatedUtc)
                    .ToListAsync();
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
