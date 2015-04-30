using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mzayad.Data;
using Mzayad.Models;

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
        public async Task<Bid> GetHighestBid(int auctionId)
        {
            using (var dc = DataContext())
            {
                return await dc.Bids
                    .Where(i => i.AuctionId == auctionId)
                    .OrderByDescending(i => i.Amount)
                    .ThenByDescending(i => i.CreatedUtc)
                    .FirstOrDefaultAsync();
            }
        }
    }
}
