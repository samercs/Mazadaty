using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using OrangeJetpack.Localization;

namespace Mzayad.Services
{
    public class TrophyService : ServiceBase
    {
        public TrophyService(IDataContextFactory dataContextFactory)
            :base(dataContextFactory)
        { }

        public async Task<IEnumerable<Trophy>> GetAll(string languageCode="en")
        {
            using (var dc = new DataContext())
            {
                var trophies = await dc.Trophies.ToListAsync();
                return trophies.Localize(languageCode, i => i.Name, i => i.Description);
            }
        }

        public async Task<Trophy> GetBykey(TrophyKey key)
        {
            using (var dc = new DataContext())
            {
                return await dc.Trophies.SingleOrDefaultAsync(i => i.Key == key);
            }
        }

        public async Task<Trophy> Update(Trophy trophy)
        {
            using (var dc = new DataContext())
            {
                dc.SetModified(trophy);
                dc.Entry(trophy).Property(i => i.Key).IsModified = false;
                await dc.SaveChangesAsync();
                return trophy;
            }
        }
    }
}
