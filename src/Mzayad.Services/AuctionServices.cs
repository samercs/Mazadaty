using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Data;
using Mzayad.Models;

namespace Mzayad.Services
{
    public class AuctionServices:ServiceBase
    {
        public AuctionServices(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<Auction> Add(Auction auction)
        {
            using (var dc=DataContext())
            {
                dc.Auctions.Add(auction);
                await dc.SaveChangesAsync();
                return auction;
            }
        }
    }
}
