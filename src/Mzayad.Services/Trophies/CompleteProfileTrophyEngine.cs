using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using System.Collections.Generic;

namespace Mzayad.Services.Trophies
{
    public class CompleteProfileTrophyEngine : TrophyEngine
    {
        public CompleteProfileTrophyEngine(IDataContextFactory dataContextFactory) : base(dataContextFactory)
        {
        }

        protected override IEnumerable<TrophyKey?> TryGetEarnedTrophies(ApplicationUser user)
        {
            yield return CheckProfileComplete(user);
        }

        private TrophyKey? CheckProfileComplete(ApplicationUser user)
        {
            var lastTime = _trophyService.GetLastEarnedTrophy(TrophyKey.CompleteProfile, user.Id).Result;
            if (user.Gender != null && user.Birthdate != null && !string.IsNullOrWhiteSpace(user.AvatarUrl) && lastTime == null)
            {
                return TrophyKey.CompleteProfile;
            }

            return null;
        }
    }
}
