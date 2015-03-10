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


        public async Task<List<CategoryNotification>> AddList(List<CategoryNotification> toAdd)
        {
            using (var dc=DataContext())
            {
                foreach (var item in toAdd)
                {
                    dc.CategoryNotifications.Add(item);
                }

                await dc.SaveChangesAsync();
            }

            return toAdd;
        }

        public async Task<List<CategoryNotification>> DeleteList(List<CategoryNotification> toDelete)
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

            return toDelete;
        }
    }
}
