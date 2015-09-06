using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using OrangeJetpack.Localization;

namespace Mzayad.Services
{
    public class TrophyService : ServiceBase
    {
        public TrophyService(IDataContextFactory dataContextFactory)
            : base(dataContextFactory)
        { }

        public async Task<IEnumerable<Trophy>> GetAll(string languageCode = "en")
        {
            using (var dc = new DataContext())
            {
                var trophies = await dc.Trophies.ToListAsync();
                return trophies.Localize(languageCode, i => i.Name, i => i.Description);
            }
        }

        public async Task<Trophy> GetTrophy(TrophyKey key)
        {
            using (var dc = new DataContext())
            {
                return await dc.Trophies.SingleOrDefaultAsync(i => i.Key == key);
            }
        }

        public async Task<Trophy> GetTrophy(int trophyId)
        {
            using (var dc = new DataContext())
            {
                return await dc.Trophies.SingleOrDefaultAsync(i => i.TrophyId == trophyId);
            }
        }

        public async Task<IEnumerable<UserTrophy>> GetMostRecentByUser(string userId, string languageCode = "en")
        {
            using (var dc = new DataContext())
            {
                var trophies = await dc.UsersTrophies.Include(i => i.Trophy).Where(i => i.UserId == userId).OrderByDescending(i => i.CreatedUtc).Take(10).ToListAsync();
                trophies.ForEach(i => i.Trophy.Localize(languageCode, t => t.Name, t => t.Description));
                return trophies;//.Localize(languageCode, i => i.Trophy.Name, i => i.Trophy.Description).ToList();
            }
        }

        public async Task<Trophy> Update(Trophy trophy)
        {
            using (var dc = new DataContext())
            {
                dc.SetModified(trophy);
                dc.Entry(trophy).Property(i => i.Key).IsModified = false;
                await dc.SaveChangesAsync();
                return trophy;
            }
        }

        public async Task<bool> AddToUser(int trophyId, string userId)
        {
            using (var dc = new DataContext())
            {
                var trophy = await dc.Trophies.SingleOrDefaultAsync(i => i.TrophyId == trophyId);
                if (null != trophy)
                {
                    dc.UsersTrophies.Add(new UserTrophy()
                    {
                        TrophyId = trophy.TrophyId,
                        UserId = userId,
                        XpAwarded = trophy.XpAward
                    });
                    var user = await dc.Users.SingleOrDefaultAsync(i => i.Id == userId);
                    user.Xp += trophy.XpAward;

                    await dc.SaveChangesAsync();
                    return true;
                }
                return false;
            }
        }

        public async void AddToUser(IEnumerable<Trophy> trophies, string userId)
        {
            foreach (var trophy in trophies)
            {
                await AddToUser(trophy.TrophyId, userId);
            }
        }

        public async Task<UserTrophy> GetLastEarnedTrophy(TrophyKey key, string userId)
        {
            using (var dc = new DataContext())
            {
                return await dc.UsersTrophies.Where(i => i.UserId == userId && i.Trophy.Key == key)
                        .OrderByDescending(i => i.CreatedUtc)
                        .FirstOrDefaultAsync();
            }
        }
    }
}
