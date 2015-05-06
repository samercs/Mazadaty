using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Data;
using Mzayad.Models;

namespace Mzayad.Services
{
    public class WishListService : ServiceBase
    {

        public WishListService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<IEnumerable<WishList>> GetByUser(string userId)
        {
            using (var dc=DataContext())
            {
                return await dc.WishLists.Include(i => i.User).Where(i => i.UserId == userId).ToListAsync();
            }
        }

        public async Task<WishList> Add(WishList wishList)
        {
            using (var dc = DataContext())
            {
                dc.WishLists.Add(wishList);
                await dc.SaveChangesAsync();
                return wishList;
            }
        }

        public async Task<WishList> GetById(int id)
        {
            using (var dc = DataContext())
            {
                return await dc.WishLists.SingleOrDefaultAsync(i => i.WishListId == id);
            }
        }

        public async Task Delete(WishList wishlist)
        {
            using (var dc = DataContext())
            {
                dc.WishLists.Attach(wishlist);
                dc.WishLists.Remove(wishlist);
                await dc.SaveChangesAsync();

            }
        }

        public async Task<IEnumerable<WishListAdminModel>> GetGroupBy(DateTime? startDate,DateTime? endDate)
        {
            using (var dc = DataContext())
            {
                var query = dc.WishLists.AsQueryable();
                if (startDate.HasValue)
                {
                    query = query.Where(i => i.CreatedUtc >= startDate.Value);
                }
                if (endDate.HasValue)
                {
                    query = query.Where(i => i.CreatedUtc <= endDate.Value);
                }

                return await query.GroupBy(i => i.NameNormalized).Select(group => new WishListAdminModel()
                {
                    Name = group.Key,
                    Count = group.Count()
                })
                .OrderByDescending(j=>j.Count)
                .ToListAsync();

            }
        }

        public async Task<IEnumerable<WishList>> GetByNameNormalized(string name)
        {
            using (var dc = DataContext())
            {
                return await dc.WishLists.Where(i => i.NameNormalized.Equals(name)).ToListAsync();
            }
        }

        public async Task<IEnumerable<WishList>>  EditRange(IEnumerable<WishList> wishlist)
        {
            using (var dc = DataContext())
            {
                foreach (var item in wishlist)
                {
                    dc.SetModified(item);
                }
                await dc.SaveChangesAsync();
                return wishlist;
            }
        }
    }
}
