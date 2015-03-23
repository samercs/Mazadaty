using Mzayad.Data;
using Mzayad.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mzayad.Models.Enum;

namespace Mzayad.Services
{
    public class AuctionService : ServiceBase
    {
        public AuctionService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
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
        public async Task<IEnumerable<Auction>> GetCurrentAuctions()
        {
            using (var dc = DataContext())
            {
                return await dc.Auctions
                    .Where(i => i.Status == AuctionStatus.Public)
                    .OrderBy(i => i.StartUtc)
                    .ToListAsync();
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
    }
}
