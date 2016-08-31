using Mzayad.Data;
using Mzayad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;


namespace Mzayad.Services
{
    public class SplashAdService : ServiceBase
    {
        public SplashAdService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
        }

        public async Task<IEnumerable<SplashAd>> GetAll()
        {
            using (var dc = DataContext())
            {
                return await dc.SplashAds.OrderBy(i => i.SortOrder).ToListAsync();
            }
        }

        public async Task<SplashAd> GetById(int splashAdId)
        {
            using (var dc = DataContext())
            {
                return await dc.SplashAds.SingleOrDefaultAsync(i => i.SplashAdId == splashAdId);
            }
        }

        public async Task<SplashAd> Add(SplashAd splashAd)
        {
            using (var dc = DataContext())
            {
                var sortOrder = 0d;
                var allSplashAds = await dc.SplashAds.OrderBy(i => i.SortOrder).ToListAsync();

                if (allSplashAds.Any())
                {
                    sortOrder = allSplashAds.Last().SortOrder + 1;
                }

                splashAd.SortOrder = sortOrder;
                splashAd.Weight = 1;

                dc.SplashAds.Add(splashAd);
                await dc.SaveChangesAsync();

                return splashAd;
            }
        }

        public async Task<SplashAd> Update(SplashAd splashAd)
        {
            using (var dc = DataContext())
            {
                dc.SetModified(splashAd);
                await dc.SaveChangesAsync();
                return splashAd;
            }
        }

        public async Task Delete(SplashAd splashAd)
        {
            using (var dc = DataContext())
            {
                dc.SplashAds.Attach(splashAd);
                dc.SplashAds.Remove(splashAd);
                await dc.SaveChangesAsync();
            }
        }

        public async Task UpdateWeights(IEnumerable<SplashAd> splashAds)
        {
            using (var dc = DataContext())
            {
                foreach (var splashAd in splashAds)
                {
                    var tmp = await dc.SplashAds.SingleOrDefaultAsync(i => i.SplashAdId == splashAd.SplashAdId);
                    if (tmp != null)
                    {
                        tmp.Weight = splashAd.Weight;
                    }
                }

                await dc.SaveChangesAsync();
            }
        }

    }
}
