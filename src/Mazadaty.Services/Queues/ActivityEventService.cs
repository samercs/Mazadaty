using Mazadaty.Data;
using Mazadaty.Models;
using System.Threading.Tasks;

namespace Mazadaty.Services.Activity
{
    public class ActivityEventService : ServiceBase
    {
        public ActivityEventService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        public async Task<ActivityEvent> AddActivity(ActivityEvent activityEvent)
        {
            using (var dc = DataContext())
            {
                dc.ActivityEvents.Add(activityEvent);
                await dc.SaveChangesAsync();

                return activityEvent;
            }
        } 
    }
}
