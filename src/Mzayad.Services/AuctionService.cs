﻿using System;
using Mzayad.Data;
using Mzayad.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mzayad.Models.Enum;
using OrangeJetpack.Localization;
using Mzayad.Core.Extensions;

namespace Mzayad.Services
{
    public class AuctionService : ServiceBase
    {
        private readonly BidService _bidService;
        private readonly OrderService _orderService;
        
        public AuctionService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
            _bidService = new BidService(dataContextFactory);
            _orderService=new OrderService(dataContextFactory);
        }

        public async Task<Auction> Add(Auction auction)
        {
            using (var dc = DataContext())
            {
                dc.Auctions.Add(auction);
                await dc.SaveChangesAsync();

                return await GetAuction(dc, auction.AuctionId);
            }
        }

        /// <summary>
        /// Gets a list of recent and upcoming public auctions.
        /// </summary>
        public async Task<IReadOnlyCollection<Auction>> GetCurrentAuctions(string language = "en")
        {
            using (var dc = DataContext())
            {
                var auctions = await dc.Auctions
                    .Where(i => i.Status != AuctionStatus.Hidden)
                    .Include(i => i.Product.ProductImages)
                    .Include(i => i.Product.ProductSpecifications.Select(j => j.Specification))
                    .Include(i => i.WonByUser)
                    .OrderBy(i => i.Status)
                    .ThenBy(i => i.StartUtc)
                    .ToListAsync();

                return LocalizeAuctions(language, auctions);
            }
        }

        public async Task<IReadOnlyCollection<Auction>> GetLiveAuctions(string language)
        {
            using (var dc = DataContext())
            {
                var auctions = await GetAuctionsQuery(dc, AuctionStatus.Public)
                    .OrderBy(i => i.StartUtc)
                    .ToListAsync();

                return LocalizeAuctions(language, auctions);
            }
        }

        /// <summary>
        /// Gets a collection of public auctions having a collection of AuctionIds.
        /// </summary>
        public async Task<IEnumerable<Auction>> GetLiveAuctions(IEnumerable<int> auctionIds)
        {
            using (var dc = DataContext())
            {
                return await GetAuctionsQuery(dc, AuctionStatus.Public)
                    .Where(i => auctionIds.Contains(i.AuctionId))
                    .ToListAsync();
            }
        }

        public async Task<IReadOnlyCollection<Auction>> GetClosedAuctions(string language, int count)
        {
            using (var dc = DataContext())
            {
                var auctions = await GetAuctionsQuery(dc, AuctionStatus.Closed)
                    .Take(count)
                    .OrderByDescending(i => i.ClosedUtc)
                    .ToListAsync();

                return LocalizeAuctions(language, auctions);
            }
        }

        private static IQueryable<Auction> GetAuctionsQuery(IDataContext dc, AuctionStatus auctionStatus)
        {
            return dc.Auctions
                .Where(i => i.Status == auctionStatus)
                .Include(i => i.Product.ProductImages)
                //.Include(i => i.Product.ProductSpecifications.Select(j => j.Specification))
                .Include(i => i.WonByUser)
                .Include(i => i.Bids.Select(j => j.User));
        }

        private static IReadOnlyCollection<Auction> LocalizeAuctions(string language, List<Auction> auctions)
        {
            foreach (var product in auctions.Select(i => i.Product).Distinct())
            {
                LocalizeProduct(product, language);
            }

            return auctions.Localize(language, i => i.Title).ToList();
        }

        private static void LocalizeProduct(Product product, string language)
        {
            // order product images
            product.ProductImages = product.ProductImages.OrderBy(i => i.SortOrder).ToList();

            // localize products
            product.Localize(language, i => i.Name, i => i.Description);

            //// localize specifications
            //foreach (var specification in product.ProductSpecifications)
            //{
            //    specification.Localize(language, i => i.Value);
            //    specification.Specification.Localize(language, i => i.Name);
            //}
        }

        public async Task<IReadOnlyCollection<Auction>> GetAllAuctions(string search = null)
        {
            using (var dc = DataContext())
            {
                if (!string.IsNullOrEmpty(search))
                {
                    return await dc.Auctions.Include(i => i.Product).Where(i => i.Product.Name.Contains(search)).OrderByDescending(i => i.StartUtc).ToListAsync();

                }
                
                return await dc.Auctions.Include(i => i.Product).OrderByDescending(i => i.StartUtc).ToListAsync();
            }
        }

        public async Task<IReadOnlyCollection<Auction>> GetAuctionsWon(string userId, string language)
        {
            using (var dc = DataContext())
            {
                var auctions = await dc.Auctions
                    .Where(i => i.WonByUserId == userId)
                    .Include(i => i.Product.ProductImages)
                    .OrderByDescending(i => i.ClosedUtc)
                    .ToListAsync();

                return LocalizeAuctions(language, auctions);
            }
        }

        public async Task<Auction> GetAuction(int auctionId, string language = null)
        {
            using (var dc = DataContext())
            {
                var auction = await GetAuction(dc, auctionId);

                if (language != null)
                {
                    LocalizeProduct(auction.Product, language);
                    auction.Localize(language, i => i.Title);
                }

                return auction;
            }
        }

        private static async Task<Auction> GetAuction(IDataContext dc, int auctionId)
        {
            return await dc.Auctions
                .Include(i => i.Product.Categories)
                .Include(i => i.Product.ProductImages)
                .Include(i => i.Product.ProductSpecifications.Select(j => j.Specification))
                .SingleOrDefaultAsync(i => i.AuctionId == auctionId);
        }


        public async Task<Auction> Update(Auction auction)
        {
            using (var dc = DataContext())
            {
                var product = await dc.Products.SingleOrDefaultAsync(i => i.ProductId == auction.ProductId);
                auction.Product = product;
                dc.Auctions.Attach(auction);
                dc.SetModified(auction);
                await dc.SaveChangesAsync();

                return await GetAuction(dc, auction.AuctionId);
            }
        }

        /// <summary>
        /// Closes an auction and records the highest bid.
        /// </summary>
        public async Task<Order> CloseAuction(int auctionId)
        {
            using (var dc = DataContext())
            {
                var auction = await dc.Auctions
                    .Include(i => i.Product)
                    .SingleAsync(i => i.AuctionId == auctionId);
                    
                auction.ClosedUtc = DateTime.UtcNow;
                auction.Status = AuctionStatus.Closed;

                var winningBid = await _bidService.GetWinningBid(auctionId);
                if (winningBid != null)
                {
                    auction.WonByUserId = winningBid.UserId;
                    auction.WonAmount = winningBid.Amount;
                    auction.WonByBidId = winningBid.BidId;           
                }

                await dc.SaveChangesAsync();

                if (winningBid != null)
                {
                    var order = await _orderService.CreateOrderForAuction(auction, winningBid);
                    return order;
                }
                
                return null;
            }
        }

        public async Task<int> CountAuctionsWon(string userId,DateTime? from = null)
        {
            using (var dc = DataContext())
            {
                return await dc.Auctions
                    .CountAsync(i => i.WonByUserId == userId && (
                                                                (from.HasValue && i.CreatedUtc >= from.Value)
                                                                ||
                                                                !from.HasValue));
            }
        }

        /// <summary>
        /// Gets the total amount of consecutive days that a user has bid since today.
        /// </summary>
        public async Task<int> GetConsecutiveWonDays(string userId)
        {
            using (var dc = DataContext())
            {
                var wonAuctionsDates = await dc.Auctions
                    .Where(i => i.WonByUserId == userId && i.ClosedUtc.HasValue)
                    .OrderByDescending(i => i.ClosedUtc)
                    .Select(i => i.ClosedUtc.Value)
                    .ToListAsync();

                return wonAuctionsDates.Consecutive();
            }
        }
    }
}
