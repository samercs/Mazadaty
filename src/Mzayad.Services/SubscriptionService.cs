using System;
using Mzayad.Data;
using Mzayad.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mzayad.Models.Enum;
using OrangeJetpack.Localization;

namespace Mzayad.Services
{
    public class SubscriptionService : ServiceBase
    {
        public SubscriptionService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        {
        }

        public async Task<IReadOnlyCollection<Subscription>> GetActiveSubscriptions(string languageCode)
        {
            using (var dc = DataContext())
            {
                var subscriptions = await dc.Subscriptions
                    .Where(i => i.Status == SubscriptionStatus.Active)
                    .Where(i => i.ExpirationUtc > DateTime.UtcNow)
                    .OrderBy(i => i.SortOrder)
                    .ThenBy(i => i.ExpirationUtc)
                    .ToListAsync();

                return subscriptions.Localize(languageCode, i => i.Name).ToList();
            }
        }

        public async Task<IReadOnlyCollection<Subscription>> GetAll(string languageCode = null)
        {
            using (var dc = DataContext())
            {
                var subscriptions = await dc.Subscriptions
                    .OrderBy(i => i.SortOrder)
                    .ThenBy(i => i.ExpirationUtc)
                    .ToListAsync();

                if (!string.IsNullOrWhiteSpace(languageCode))
                {
                    subscriptions = subscriptions.Localize(languageCode, i => i.Name).ToList();
                }

                return subscriptions;
            }
        }

        public async Task<Subscription> GetById(int subscriptionId)
        {
            using (var dc = DataContext())
            {
                return await dc.Subscriptions.SingleOrDefaultAsync(i => i.SubscriptionId == subscriptionId);
            }
        }

        public async Task<Subscription> Add(Subscription subscription)
        {
            using (var dc = DataContext())
            {
                dc.Subscriptions.Add(subscription);
                await dc.SaveChangesAsync();
                return subscription;
            }
        }

        public async Task<Subscription> UpdateSubscription(Subscription subscription)
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
