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
    }
}
