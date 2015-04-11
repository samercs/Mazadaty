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
                return await dc.Avatars.OrderBy(i=>i.SortOrder).ToArrayAsync();
            }
        }

        public async Task<Avatar> Add(Avatar avatar)
        {
            using (var dc=DataContext())
            {
                double sortOrder = 0.0;
                var allAvatar = await dc.Avatars.OrderBy(i=>i.SortOrder).ToArrayAsync();
                if (allAvatar.Any())
                {
                    sortOrder = allAvatar.Last().SortOrder + 1;    
                }
                avatar.SortOrder = sortOrder;
                dc.Avatars.Add(avatar);
                await dc.SaveChangesAsync();
                return avatar;

            }
        }

        public async Task<Avatar> GetById(int id)
        {
            using (var dc=DataContext())
            {
                return await dc.Avatars.SingleOrDefaultAsync(i => i.AvatarId == id);
            }
        }

        public async Task<Avatar> Delete(Avatar avatar)
        {
            using (var dc = DataContext())
            {
                dc.Avatars.Attach(avatar);
                dc.Avatars.Remove(avatar);
                await dc.SaveChangesAsync();
                return avatar;
            }
        }

        public async Task<Avatar> Update(Avatar avatar)
        {
            using (var dc=DataContext())
            {
                dc.SetModified(avatar);
                await dc.SaveChangesAsync();
                return avatar;
            }
        }
    }
}
