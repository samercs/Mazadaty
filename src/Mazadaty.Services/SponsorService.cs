using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mazadaty.Data;
using Mazadaty.Models;
using OrangeJetpack.Localization;

namespace Mazadaty.Services
{
    public class SponsorService :ServiceBase
    {
        public SponsorService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<IEnumerable<Sponsor>> GetAll()
        {
            using (var dc=DataContext())
            {
                return await dc.Sponsors.OrderBy(i => i.Name).ToListAsync();
            }
        }

        public async Task<Sponsor> Add(Sponsor sponsor)
        {
            using (var dc=DataContext())
            {
                dc.Sponsors.Add(sponsor);
                await dc.SaveChangesAsync();
                return sponsor;
            }
        }

        public async Task<Sponsor> GetById(int id)
        {
            using (var dc=DataContext())
            {
                return await dc.Sponsors.SingleOrDefaultAsync(i => i.SponsorId == id);
            }
        }

        public async Task<Sponsor> Save(Sponsor sponsor)
        {
            using (var dc = DataContext())
            {
                dc.SetModified(sponsor);
                await dc.SaveChangesAsync();
                return sponsor;
            }
        }

        public async Task Delete(Sponsor sponsor)
        {
            using (var dc = DataContext())
            {
                var sponsoredProducts = dc.Products.Where(i => i.SponsorId == sponsor.SponsorId);
                foreach (var product in sponsoredProducts)
                {
                    product.Sponsor = null;
                }
                
                dc.Sponsors.Attach(sponsor);
                dc.Sponsors.Remove(sponsor);
                
                await dc.SaveChangesAsync();           
            }
        }
    }
}
