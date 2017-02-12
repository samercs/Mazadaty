using Mzayad.Data;
using Mzayad.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Mzayad.Services
{
    public class IslamicCalendarService : ServiceBase
    {
        public IslamicCalendarService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        { }

        public async Task<IEnumerable<IslamicCalendar>> GetAll()
        {
            using (var dc = new DataContext())
            {
                return await dc.IslamicCalendars.ToListAsync();
            }
        }

        public async Task<IslamicCalendar> GetByYear(int hijriYear)
        {
            using (var dc = new DataContext())
            {
                return await dc.IslamicCalendars.SingleOrDefaultAsync(i => i.HijriYear == hijriYear);
            }
        }

        public async Task<IslamicCalendar> GetById(int id)
        {
            using (var dc = new DataContext())
            {
                return await dc.IslamicCalendars.SingleOrDefaultAsync(i => i.IslamicCalendarId == id);
            }
        }

        public async Task<IslamicCalendar> GetByDate(DateTime date)
        {
            using (var dc = new DataContext())
            {
                var d = date.Date;

                return await dc.IslamicCalendars
                            .FirstOrDefaultAsync(i => DbFunctions.TruncateTime(i.NewYear) == d
                                                || d >= DbFunctions.TruncateTime(i.EidFetrFrom) && d <= DbFunctions.TruncateTime(i.EidFetrTo)
                                                || d >= DbFunctions.TruncateTime(i.EidAdhaFrom) && d <= DbFunctions.TruncateTime(i.EidAdhaTo));
            }
        }

        public async Task<IslamicCalendar> Update(IslamicCalendar entity)
        {
            using (var dc = new DataContext())
            {
                dc.IslamicCalendars.Attach(entity);
                dc.SetModified(entity);
                await dc.SaveChangesAsync();
                return entity;
            }
        }
    }
}
