using System;
using System.Linq;
using Mazadaty.Data;
using Mazadaty.Models;
using System.Threading.Tasks;
using System.Data.Entity;
using Mazadaty.Core.Extensions;

namespace Mazadaty.Services
{
    public class SessionLogService : ServiceBase
    {
        public SessionLogService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        { }

        public SessionLog Insert(SessionLog sessionLog)
        {
            using (var dc = new DataContext())
            {
                dc.SessionLogs.Add(sessionLog);
                dc.SaveChanges();
                return sessionLog;
            }
        }
        public SessionLog GetLastVisitBeforeToday(string userId)
        {
            using (var dc = new DataContext())
            {
                return dc.SessionLogs.Last(i => i.UserId == userId && i.CreatedUtc.Date != DateTime.UtcNow.Date);
            }
        }

        /// <summary>
        /// Gets the total amount of consecutive days that a user has bid since today.
        /// </summary>
        public async Task<int> GetConsecutiveVisitDays(string userId)
        {
            using (var dc = DataContext())
            {
                var userSessionDates = await dc.SessionLogs
                    .Where(i => i.UserId == userId)
                    .OrderByDescending(i => i.CreatedUtc)
                    .Select(i => i.CreatedUtc)
                    .ToListAsync();

                return userSessionDates.Consecutive();
            }
        }
    }
}
