﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Data;
using Mzayad.Models;
using OrangeJetpack.Localization;

namespace Mzayad.Services
{
    public class AuctionServices : ServiceBase
    {
        public AuctionServices(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
        }

        public async Task<Auction> Add(Auction auction)
        {
            using (var dc = DataContext())
            {
                dc.Auctions.Add(auction);
                await dc.SaveChangesAsync();
                return auction;
            }
        }

        public async Task<IEnumerable<Auction>> GetAuctions( string search = null)
        {
            using (var dc = DataContext())
            {


                if (!string.IsNullOrEmpty(search))
                {
                    return await dc.Auctions.Include(i => i.Product).Where(i => i.Product.Name.Contains(search)).OrderByDescending(i=>i.StartUtc).ToListAsync();
                    
                }
                else
                {
                    return await dc.Auctions.Include(i => i.Product).OrderByDescending(i => i.StartUtc).ToListAsync();
                }
            }
        }

        public async Task<Auction> GetAuction(int id)
        {
            using (var dc=DataContext())
            {
                return await dc.Auctions.Include(i=>i.Product).SingleOrDefaultAsync(i => i.AuctionId == id);
            }
        }

        public async Task<Auction> Update(Auction auction)
        {
            using (var dc=DataContext())
            {
                
                var product =await dc.Products.SingleOrDefaultAsync(i => i.ProductId == auction.ProductId);
                auction.Product = product;
                dc.Auctions.Attach(auction);
                dc.SetModified(auction);
                await dc.SaveChangesAsync();
                return auction;
            }
        }
    }
}
