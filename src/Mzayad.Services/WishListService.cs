using System;
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
    }
}
