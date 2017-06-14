using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mzayad.Services.Trophies
{
    public class CompleteProfileTrophyEngine : TrophyEngine
    {
        public CompleteProfileTrophyEngine(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        protected async Task<IReadOnlyCollection<TrophyKey>> TryGetEarnedTrophies(ApplicationUser user)
        {
            return new List<TrophyKey>();

            //yield return CheckProfileComplete(user);
        }

        private TrophyKey? CheckProfileComplete(ApplicationUser user)
        {
            var lastTime = TrophyService.GetLastEarnedTrophy(TrophyKey.CompleteProfile, user.Id).Result;
            if (user.Gender != null && user.Birthdate != null && !string.IsNullOrWhiteSpace(user.AvatarUrl) && lastTime == null)
            {
                return TrophyKey.CompleteProfile;
            }

            return null;
        }
    }
}
