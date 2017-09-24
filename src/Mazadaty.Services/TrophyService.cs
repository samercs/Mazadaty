using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Mazadaty.Data;
using Mazadaty.Models;
using Mazadaty.Models.Enum;
using Mazadaty.Services.Identity;
using OrangeJetpack.Localization;

namespace Mazadaty.Services
{
    public class TrophyService : ServiceBase
    {
        private readonly UserService _userService;

        public TrophyService(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
            _userService = new UserService(DataContextFactory);
        }

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

        public async Task AwardTrophyToUser(string userId, TrophyKey trophyKey)
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

                await dc.SaveChangesAsync();

                await _userService.AddXp(userId, trophy.XpAward);
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
