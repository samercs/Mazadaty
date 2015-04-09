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
    public class AvatarService : ServiceBase
    {
        public AvatarService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<IEnumerable<Avatar>> GetAll()
        {
            using (var dc=DataContext())
            {
                return await dc.Avatars.ToArrayAsync();
            }
        }

        public async Task<Avatar> Add(Avatar avatar)
        {
            using (var dc=DataContext())
            {

                avatar.SortOrder = dc.Avatars.OrderBy(i=>i.SortOrder).Last().SortOrder + 1;
                dc.Avatars.Add(avatar);
                await dc.SaveChangesAsync();
                return avatar;

            }
        }
    }
}
