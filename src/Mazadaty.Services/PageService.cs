using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mazadaty.Data;
using Mazadaty.Models;

namespace Mazadaty.Services
{
    public class PageService: ServiceBase
    {
        public PageService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<Page> GetByTag(string tag)
        {
            using (var dc = DataContext())
            {
                return await dc.Pages.SingleOrDefaultAsync(i => i.PageTag.Equals(tag, StringComparison.OrdinalIgnoreCase));
            }
        }

        public async Task<IReadOnlyCollection<Page>> GetPages()
        {
            using (var dc = DataContext())
            {
                return await dc.Pages.Where(i => i.IsDeleted == false).ToListAsync();
            }
        }

        public async Task<Page> AddPage(Page page)
        {
            using (var dc = DataContext())
            {
                dc.Pages.Add(page);
                await dc.SaveChangesAsync();
                return page;
            }
        }

        public async Task<Page> GetById(int id)
        {
            using (var dc = DataContext())
            {
                return await dc.Pages.SingleOrDefaultAsync(i => i.IsDeleted == false && i.PageId == id);
            }
        }


        public async Task<Page> Update(Page page)
        {
            using (var dc = DataContext())
            {
                dc.SetModified(page);
                await dc.SaveChangesAsync();
                return page;
            }

        }

        public async Task Delete(Page page)
        {
            using (var dc = DataContext())
            {
                page.IsDeleted = true;
                dc.SetModified(page);
                await dc.SaveChangesAsync();
            }
        }
    }
}
