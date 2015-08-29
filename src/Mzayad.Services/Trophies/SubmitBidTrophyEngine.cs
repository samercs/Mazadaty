using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enum;
using Mzayad.Services.Identity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mzayad.Services.Trophies
{
    public class SubmitBidTrophyEngine : ITrophyEngine
    {
        private readonly UserService _userService;
        private readonly TrophyService _trophyService;

        public SubmitBidTrophyEngine(IDataContextFactory dataContextFactory)
        {
            _userService = new UserService(dataContextFactory);
            _trophyService = new TrophyService(dataContextFactory);
        }

        public IEnumerable<Trophy> EvaluateActivityForPossibleTrophy(ApplicationUser user)
        {
            yield return CheckBidOnNewYears(user.Id);
            // bid on Eid
            // bid on birthday
        }

        private Trophy CheckBidOnNewYears(string userId)
        {
            if (DateTime.Now.Month != 1 || DateTime.Now.Day != 1)
            {
                return null;
            }
            var userTrophy = _trophyService.GetLastEarnedTrophy(TrophyKey.BidOnNewYear, userId).Result;
            if (userTrophy.CreatedUtc.Year == DateTime.Now.Year)
            {
                return null;
            }
            return new Trophy() { TrophyId = (int)TrophyKey.BidOnNewYear };
        }
    }
}
