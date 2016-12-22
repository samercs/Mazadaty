using Mzayad.Data;
using Mzayad.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Mzayad.Services
{
    public class AvatarService : ServiceBase
    {
        public AvatarService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
        }

        public async Task<IReadOnlyCollection<Avatar>> GetAll()
        {
            using (var dc = DataContext())
            {
                return await dc.Avatars.OrderBy(i => i.SortOrder).ToListAsync();
            }
        }

        public async Task<Avatar> GetById(int id)
        {
            using (var dc = DataContext())
            {
                return await dc.Avatars.SingleOrDefaultAsync(i => i.AvatarId == id);
            }
        }

        public async Task<Avatar> Add(Avatar avatar)
        {
            using (var dc = DataContext())
            {
                var sortOrder = 0.0;
                var allAvatars = await dc.Avatars.OrderBy(i => i.SortOrder).ToListAsync();
                if (allAvatars.Any())
                {
                    sortOrder = allAvatars.Last().SortOrder + 1;
                }
                avatar.SortOrder = sortOrder;

                dc.Avatars.Add(avatar);
                await dc.SaveChangesAsync();

                return avatar;
            }
        }

        public async Task<Avatar> Update(Avatar avatar)
        {
            using (var dc = DataContext())
            {
                dc.SetModified(avatar);
                await dc.SaveChangesAsync();
                return avatar;
            }
        }

        public async Task Delete(Avatar avatar)
        {
            using (var dc = DataContext())
            {
                dc.Avatars.Attach(avatar);
                dc.Avatars.Remove(avatar);
                await dc.SaveChangesAsync();
            }
        }

        public async Task<Avatar> Save(Avatar avatar)
        {
            using (var dc = DataContext())
            {
                dc.SetModified(avatar);
                await dc.SaveChangesAsync();
                return avatar;
            }
        }
    }
}
