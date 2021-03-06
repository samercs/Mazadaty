using Mazadaty.Core.Extensions;
using Mazadaty.Data;
using Mazadaty.Models;
using Mazadaty.Models.Enums;
using Mazadaty.Services.Activity;
using OrangeJetpack.Localization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Linq.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Mazadaty.Core.Exceptions;
using Mazadaty.Services.Queues;

namespace Mazadaty.Services
{
    public class AuctionService : ServiceBase
    {
        private readonly BidService _bidService;
        private readonly OrderService _orderService;

        public AuctionService(IDataContextFactory dataContextFactory, IQueueService queueService) : base(dataContextFactory)
        {
            _bidService = new BidService(dataContextFactory, queueService);
            _orderService = new OrderService(dataContextFactory);
        }

        public async Task<Auction> Add(Auction auction)
        {
            using (var dc = DataContext())
            {
                var product = await dc.Products.SingleOrDefaultAsync(i => i.ProductId == auction.ProductId);
                if (product == null)
                {
                    throw new ArgumentException($"Can't find product with ProductId = {auction.ProductId}");
                }

                //check product quntity
                int neededQuntity = 1;
                if (auction.BuyNowEnabled && auction.BuyNowQuantity.HasValue)
                {
                    neededQuntity += auction.BuyNowQuantity.Value;
                }
                if (product.Quantity < neededQuntity)
                {
                    throw new InsufficientQuantity($"quantity for the product ({product.ProductId}) is {product.Quantity} < auction quantity ({neededQuntity})");
                }
                dc.Auctions.Add(auction);

                product.Quantity -= neededQuntity;
                dc.SetModified(product);

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
                    .Where(i => i.IsDeleted == false)
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
                    .ToListAsync();

                auctions = auctions
                    .Where(i => i.IsLive())
                    .OrderBy(i => i.StartUtc.AddMinutes(i.Duration)).ToList();

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
                return await dc.Auctions
                    .Include(i => i.Product.ProductImages)
                    .Include(i => i.Product.Sponsor)
                    .Include(i => i.Bids.Select(j => j.User))
                    .Where(i => i.Status == AuctionStatus.Public)
                    .Where(i => i.IsDeleted == false)
                    .Where(i => auctionIds.Contains(i.AuctionId))
                    .ToListAsync();
            }
        }

        public async Task<IReadOnlyCollection<Auction>> GetClosedAuctions(string language, int count)
        {
            using (var dc = DataContext())
            {

                var auctions = await GetAuctionsQuery(dc, AuctionStatus.Closed)
                    .Where(i => DbFunctions.DiffDays(i.StartUtc, DateTime.UtcNow) <= 7)
                    .OrderByDescending(i => i.ClosedUtc)
                    .Take(count)
                    .ToListAsync();

                return LocalizeAuctions(language, auctions);
            }
        }

        public async Task<IReadOnlyCollection<Auction>> GetUpcomingAuctions(string language, int count)
        {
            using (var dc = DataContext())
            {
                var auctions = await GetAuctionsQuery(dc, AuctionStatus.Public)
                    .Where(i => i.StartUtc > DateTime.UtcNow)
                    .Take(count)
                    .ToListAsync();

                auctions = auctions.OrderBy(i => i.StartUtc.AddMinutes(i.Duration)).ToList();
                return LocalizeAuctions(language, auctions);
            }
        }

        public async Task<IReadOnlyCollection<Auction>> GetBuyNowAuctions(string language, string search = null, int? categoryId = null)
        {
            using (var dc = DataContext())
            {
                var query = dc.Auctions
                    .Include(i => i.Product.ProductImages)
                    .Include(i => i.Product.Sponsor)
                    .Include(i => i.Product.Categories)
                    .Where(i => !i.IsDeleted)
                    .Where(i => i.BuyNowEnabled)
                    .Where(i => i.BuyNowQuantity > 0)
                    .Where(i => i.Status == AuctionStatus.Closed)
                    .Where(i => DbFunctions.DiffDays(i.StartUtc, DateTime.UtcNow) <= 7);

                if (!string.IsNullOrWhiteSpace(search))
                {
                    query = query.Where(i => i.Product.Name.Contains(search) || i.Product.Description.Contains(search));
                }

                if (categoryId.HasValue)
                {
                    query = query.Where(i => i.Product.Categories.Select(j => j.CategoryId).Contains(categoryId.Value));
                }

                var auctions = await query.ToListAsync();

                return LocalizeAuctions(language, auctions);
            }
        }

        private static IQueryable<Auction> GetAuctionsQuery(IDataContext dc, AuctionStatus auctionStatus)
        {
            return dc.Auctions
                .Where(i => i.Status == auctionStatus)
                .Where(i => i.IsDeleted == false)
                .Include(i => i.Product.ProductImages)
                //.Include(i => i.Product.ProductSpecifications.Select(j => j.Specification))
                .Include(i => i.WonByUser)
                .Include(i => i.Product.Sponsor);
            //.Include(i => i.Bids.Select(j => j.User));
        }

        private static IReadOnlyCollection<Auction> LocalizeAuctions(string language, List<Auction> auctions)
        {
            foreach (var product in auctions.Select(i => i.Product).Distinct())
            {
                LocalizeProduct(product, language);
            }

            return auctions.Localize<Auction>(language, i => i.Title).ToList();
        }

        private static void LocalizeProduct(Product product, string language)
        {
            // order product images
            product.ProductImages = product.ProductImages.OrderBy(i => i.SortOrder).ToList();

            // localize products
            product.Localize(language, i => i.Name, i => i.Description);

            product.Sponsor?.Localize(language, i => i.Name);

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
                var auctions = dc.Auctions
                    .Include(i => i.Product)
                    .Where(i => i.IsDeleted == false);

                if (!string.IsNullOrEmpty(search))
                {

                    auctions = auctions
                        .Where(i => i.Product.Name.Contains(search));
                }

                return await auctions
                    .OrderByDescending(i => i.StartUtc)
                    .ToListAsync();
            }
        }

        public async Task<IReadOnlyCollection<Auction>> GetAuctionsWon(string userId, string language)
        {
            using (var dc = DataContext())
            {
                var auctions = await dc.Auctions
                    .Where(i => i.WonByUserId == userId)
                    .Where(i => i.IsDeleted == false)
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

                    if (auction.Product.ProductSpecifications.Any())
                    {
                        foreach (var specification in auction.Product.ProductSpecifications)
                        {
                            specification.Localize(language, i => i.Value);
                            specification.Specification.Localize(language, i => i.Name);
                        }
                    }
                }

                return auction;
            }
        }

        public async Task<Auction> GetAuctionById(int auctionId)
        {
            using (var dc = DataContext())
            {
                return await dc.Auctions
                     .Include(i => i.Product)
                     .SingleOrDefaultAsync(i => i.AuctionId == auctionId);
            }
        }

        private static async Task<Auction> GetAuction(IDataContext dc, int auctionId)
        {
            return await dc.Auctions
                .Where(i => i.IsDeleted == false)
                .Include(i => i.Product.Categories)
                .Include(i => i.Product.ProductImages)
                .Include(i => i.Product.Sponsor)
                .Include(i => i.Product.ProductSpecifications.Select(j => j.Specification))
                .SingleOrDefaultAsync(i => i.AuctionId == auctionId);
        }

        public async Task<Auction> Update(Auction auction)
        {
            using (var dc = DataContext())
            {
                var product = await dc.Products.SingleOrDefaultAsync(i => i.ProductId == auction.ProductId);
                var oldAuction = await dc.Auctions.AsNoTracking().SingleOrDefaultAsync(i => i.AuctionId == auction.AuctionId);
                if (auction.BuyNowEnabled && auction.BuyNowQuantity.HasValue)
                {
                    var diff = auction.BuyNowQuantity.Value - (oldAuction.BuyNowQuantity ?? 0);
                    if (diff > 0)
                    {
                        if (diff > product.Quantity)
                        {
                            throw new InsufficientQuantity($"Insufficient Product Quantity.");
                        }
                        product.Quantity -= diff;
                        dc.SetModified(product);
                    }
                    else if (diff < 0)
                    {
                        product.Quantity += Math.Abs(diff);
                        dc.SetModified(product);
                    }

                }
                else if (!auction.BuyNowEnabled && oldAuction.BuyNowEnabled && oldAuction.BuyNowQuantity.HasValue)
                {
                    product.Quantity += oldAuction.BuyNowQuantity.Value;
                    dc.SetModified(product);
                }

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
                    return await _orderService.CreateOrderForAuction(auction, winningBid);
                }

                return null;
            }
        }

        public async Task<int> CountAuctionsWon(string userId, DateTime? from = null)
        {
            using (var dc = DataContext())
            {
                return await dc.Auctions
                    .Where(i => i.IsDeleted == false)
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
                    .Where(i => i.IsDeleted == false)
                    .Where(i => i.WonByUserId == userId && i.ClosedUtc.HasValue)
                    .OrderByDescending(i => i.ClosedUtc)
                    .Select(i => i.ClosedUtc.Value)
                    .ToListAsync();

                return wonAuctionsDates.Consecutive();
            }
        }

        public async Task DeleteAuction(int auctionId)
        {
            using (var dc = DataContext())
            {
                var auction = await dc.Auctions.Include(i => i.Product).SingleOrDefaultAsync(i => i.AuctionId == auctionId);
                if (auction == null)
                {
                    return;
                }

                auction.IsDeleted = true;
                auction.DeletedUtc = DateTime.UtcNow;

                auction.Product.Quantity += 1 + (auction.BuyNowQuantity ?? 0);
                dc.SetModified(auction.Product);

                await dc.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Auction>> GetByIds(List<int?> auctionIds)
        {
            using (var dc = DataContext())
            {
                return await dc.Auctions.Where(i => auctionIds.Contains(i.AuctionId)).ToListAsync();
            }
        }
    }
}
