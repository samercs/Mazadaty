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
    public class SubscriptionService : ServiceBase
    {
        public SubscriptionService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<IEnumerable<Subscription>> GetAll()
        {
            using (var dc=DataContext())
            {
                return await dc.Subscriptions.OrderBy(i => i.ExpirationUtc).ToListAsync();
            }
        }

        public async Task<Subscription> Add(Subscription subscription)
        {
            using (var dc=DataContext())
            {
                dc.Subscriptions.Add(subscription);
                await dc.SaveChangesAsync();
                return subscription;
            }
        }

        public async Task<Subscription> GetById(int id)
        {
            using (var dc=DataContext())
            {
                return await dc.Subscriptions.SingleOrDefaultAsync(i => i.SubscriptionId == id);
            }
        }

        public async Task<Subscription> Edit(Subscription subscription)
        {
            using (var dc = DataContext())
            {
                dc.Subscriptions.Attach(subscription);
                dc.SetModified(subscription);
                await dc.SaveChangesAsync();
                return subscription;

            }
        }
    }
}
