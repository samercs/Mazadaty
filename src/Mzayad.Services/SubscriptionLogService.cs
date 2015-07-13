using Mzayad.Data;
using Mzayad.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Mzayad.Services
{
    public class SubscriptionLogService : ServiceBase
    {
        public SubscriptionLogService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<IEnumerable<SubscriptionLog>> GetByUserId(string userId)
        {
            using (var dc = DataContext())
            {
                return await 
                    dc.SubscriptionLogs.Include(i => i.User)
                        .Include(i => i.ModifiedByUser)
                        .Where(i => i.UserId == userId)
                        .OrderBy(i=>i.CreatedUtc)
                        .ToListAsync();
            }
        }

        public async Task<IEnumerable<SubscriptionLog>> GetAll()
        {
            using (var dc = DataContext())
            {
                return await
                    dc.SubscriptionLogs.Include(i => i.User)
                        .Include(i => i.ModifiedByUser)
                        .OrderBy(i => i.CreatedUtc)
                        .ToListAsync();
            }
        }
    }
}
