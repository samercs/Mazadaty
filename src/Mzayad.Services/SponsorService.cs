using System;
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

        public async Task Delete(Sponsor model)
        {
            using (var dc = DataContext())
            {
                dc.Sponsors.Attach(model);
                dc.Sponsors.Remove(model);
                await dc.SaveChangesAsync();
                
            }
        }
    }
}
