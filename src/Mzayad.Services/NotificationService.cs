using Mzayad.Data;
using Mzayad.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Mzayad.Services
{
    public class NotificationService : ServiceBase
    {
        public NotificationService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<IEnumerable<CategoryNotification>> GetByUser(string userId)
        {
            using (var dc=DataContext())
            {
                return await dc.CategoryNotifications.Where(i => i.UserId == userId).ToListAsync();
            }
        }

        public async Task<IEnumerable<CategoryNotification>> GetByCategoryId(IEnumerable<int> categoryIds)
        {
            using (var dc = DataContext())
            {
                return await dc.CategoryNotifications.Where(i => categoryIds.Contains(i.CategoryId)).ToListAsync();
            }
        }

        public async Task AddList(IEnumerable<CategoryNotification> toAdd)
        {
            using (var dc=DataContext())
            {
                foreach (var item in toAdd)
                {
                    dc.CategoryNotifications.Add(item);
                }

                await dc.SaveChangesAsync();
            }
        }

        public async Task DeleteList(IEnumerable<CategoryNotification> toDelete)
        {
            using (var dc = DataContext())
            {
                foreach (var item in toDelete)
                {
                    dc.CategoryNotifications.Attach(item);
                    dc.CategoryNotifications.Remove(item);
                }

                await dc.SaveChangesAsync();
            }
        }
    }
}
