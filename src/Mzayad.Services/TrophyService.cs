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

        public async Task<Trophy> GetTrophy(int trophyId)
        {
            using (var dc = new DataContext())
            {
                return await dc.Trophies.SingleOrDefaultAsync(i => i.TrophyId == trophyId);
            }
        }

        public async Task<Trophy> GetTrophy(TrophyKey key)
        {
            using (var dc = new DataContext())
            {
                return await dc.Trophies.SingleOrDefaultAsync(i => i.Key == key);
            }
        }

        public async Task<IReadOnlyCollection<Trophy>> GetTrophiesByKeys(IEnumerable<TrophyKey> keys)
        {
            using (var dc = new DataContext())
            {
                return await dc.Trophies.Where(i => keys.Contains(i.Key)).ToListAsync();
            }
        }

        public async Task<IReadOnlyCollection<Trophy>> GetTrophies(string userId, string languageCode)
        {
            using (var dc = new DataContext())
            {
                var trophies = await dc.UsersTrophies
                    .Include(i => i.Trophy)
                    .Where(i => i.UserId == userId)
                    .OrderByDescending(i => i.CreatedUtc)
                    .Select(i => i.Trophy)
                    .ToListAsync();

                return trophies.Localize(languageCode, i => i.Name, i => i.Description).ToList();
            }
        }

        public async Task<IReadOnlyCollection<UserTrophy>> GetUserTrophies(string userId, string languageCode = null)
        {
            using (var dc = new DataContext())
            {
                var trophies = await dc.UsersTrophies
                    .Include(i => i.Trophy)
                    .Where(i => i.UserId == userId)
                    .OrderByDescending(i => i.CreatedUtc)
                    .ToListAsync();

                if (languageCode != null)
                {
                    trophies.ForEach(i => i.Trophy.Localize(languageCode, t => t.Name, t => t.Description));
                }

                return trophies.ToList();
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

        public async Task AwardTrophyToUser(TrophyKey trophyKey, string userId)
        {
            using (var dc = new DataContext())
            {
                var trophy = await dc.Trophies.SingleOrDefaultAsync(i => i.Key == trophyKey);

                dc.UsersTrophies.Add(new UserTrophy
                {
                    TrophyId = trophy.TrophyId,
                    UserId = userId,
                    XpAwarded = trophy.XpAward
                });

                var user = await dc.Users.SingleOrDefaultAsync(i => i.Id == userId);
                user.Xp += trophy.XpAward;

                await dc.SaveChangesAsync();
            }
        }

        public async void AwardTrophyToUser(IEnumerable<TrophyKey> trophyKeys, string userId)
        {
            foreach (var trophy in trophyKeys)
            {
                await AwardTrophyToUser(trophy, userId);
            }
        }

        public async Task<UserTrophy> GetLastEarnedTrophy(TrophyKey key, string userId)
        {
            using (var dc = new DataContext())
            {
                return await dc.UsersTrophies
                        .Where(i => i.UserId == userId)
                        .Where(i => i.Trophy.Key == key)
                        .OrderByDescending(i => i.CreatedUtc)
                        .FirstOrDefaultAsync();
            }
        }
    }
}
