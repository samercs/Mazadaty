using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mazadaty.Data;
using Mazadaty.Models;
using Mazadaty.Models.Enums;

namespace Mazadaty.Services
{
    public class BannerService: ServiceBase
    {
        public BannerService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<IEnumerable<Banner>> GetAll(BannerStatus status = BannerStatus.Public)
        {
            using (var dc = DataContext())
            {
                return await dc.Banners.Where(i => i.Status == status).ToListAsync();
            }
        }

        public async Task<Banner> Save(Banner banner)
        {
            using (var dc = DataContext())
            {
                dc.Banners.Add(banner);
                await dc.SaveChangesAsync();
                return banner;
            }
        }

        public async Task<Banner> GetById(int bannerId)
        {
            using (var dc = DataContext())
            {
                return await dc.Banners.FirstOrDefaultAsync(i => i.BannerId == bannerId);
            }
        }

        public async Task<Banner> Update(Banner banner)
        {
            using (var dc = DataContext())
            {
                dc.Banners.Attach(banner);
                dc.SetModified(banner);
                await dc.SaveChangesAsync();
                return banner;
            }
        }

        public async Task Delete(Banner banner)
        {
            using (var dc = DataContext())
            {
                dc.Banners.Attach(banner);
                dc.Banners.Remove(banner);
                await dc.SaveChangesAsync();
            }
        }
    }
}
