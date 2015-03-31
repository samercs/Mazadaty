using System;
using Mzayad.Data;
using Mzayad.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mzayad.Models.Enum;
using OrangeJetpack.Localization;

namespace Mzayad.Services
{
    public class AuctionService : ServiceBase
    {
        public AuctionService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
        }

        public async Task<Auction> Add(Auction auction, Action onAdded)
        {
            using (var dc = DataContext())
            {
                dc.Auctions.Add(auction);
                await dc.SaveChangesAsync();

                onAdded();

                return await GetAuction(dc, auction.AuctionId);
            }
        }

        /// <summary>
        /// Gets a list of recent and upcoming public auctions.
        /// </summary>
        public async Task<IEnumerable<Auction>> GetCurrentAuctions(string language = "en")
        {
            using (var dc = DataContext())
            {
                var auctions = await dc.Auctions
                    .Where(i => i.Status == AuctionStatus.Public)
                    .Include(i => i.Product.ProductImages)
                    .Include(i => i.Product.ProductSpecifications.Select(j => j.Specification))
                    .OrderBy(i => i.StartUtc)
                    .ToListAsync();

                foreach (var product in auctions.Select(i => i.Product).Distinct())
                {
                    // order product images
                    product.ProductImages = product.ProductImages.OrderBy(i => i.SortOrder).ToList();

                    // localize specifications
                    foreach (var specification in product.ProductSpecifications)
                    {
                        specification.Localize(language, i => i.Value);
                        specification.Specification.Localize(language, i => i.Name);
                    }
                }

                return auctions.Localize(language, i => i.Title);
            }
        }

        public async Task<IEnumerable<Auction>> GetAuctions(string search = null)
        {
            using (var dc = DataContext())
            {
                if (!string.IsNullOrEmpty(search))
                {
                    return await dc.Auctions.Include(i => i.Product).Where(i => i.Product.Name.Contains(search)).OrderByDescending(i => i.StartUtc).ToListAsync();

                }
                else
                {
                    return await dc.Auctions.Include(i => i.Product).OrderByDescending(i => i.StartUtc).ToListAsync();
                }
            }
        }

        /// <summary>
        /// Gets a collection of auctions having a collection of AuctionIds.
        /// </summary>
        public async Task<IEnumerable<Auction>> GetAuctions(IEnumerable<int> auctionIds)
        {
            using (var dc = DataContext())
            {
                return await dc.Auctions.Where(i => auctionIds.Contains(i.AuctionId)).ToListAsync();
            }
        }

        public async Task<Auction> GetAuction(int auctionId)
        {
            using (var dc = DataContext())
            {
                return await GetAuction(dc, auctionId);
            }
        }

        private static async Task<Auction> GetAuction(IDataContext dc, int auctionId)
        {
            return await dc.Auctions
                .Include(i => i.Product.Categories)
                .SingleOrDefaultAsync(i => i.AuctionId == auctionId);
        }


        public async Task<Auction> Update(Auction auction, Action onUpdated)
        {
            using (var dc = DataContext())
            {
                var product = await dc.Products.SingleOrDefaultAsync(i => i.ProductId == auction.ProductId);
                auction.Product = product;
                dc.Auctions.Attach(auction);
                dc.SetModified(auction);
                await dc.SaveChangesAsync();

                onUpdated();

                return await GetAuction(dc, auction.AuctionId);
            }
        }
    }
}
