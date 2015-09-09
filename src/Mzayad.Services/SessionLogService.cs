using Mzayad.Data;
using Mzayad.Models;

namespace Mzayad.Services
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
    }
}
