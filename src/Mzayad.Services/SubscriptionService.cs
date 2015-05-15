using Mzayad.Data;
using Mzayad.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using OrangeJetpack.Localization;

namespace Mzayad.Services
{
    public class SubscriptionService : ServiceBase
    {
        public SubscriptionService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<IEnumerable<Subscription>> GetAll(string languageCode = null)
        {
            using (var dc=DataContext())
            {
                var subscriptions = await dc.Subscriptions.OrderBy(i => i.ExpirationUtc).ToListAsync();

                if (!string.IsNullOrWhiteSpace(languageCode))
                {
                    subscriptions = subscriptions.Localize(languageCode, i => i.Name).ToList();
                }

                return subscriptions;
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
