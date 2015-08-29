using Mzayad.Data;
using Mzayad.Models;
using Mzayad.Models.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mzayad.Services.Trophies
{
    public static class TrophyEngineFactory
    {
        private static readonly Dictionary<ActivityType, Func<IDataContextFactory, ITrophyEngine>> Dictionary = new Dictionary<ActivityType, Func<IDataContextFactory, ITrophyEngine>>
        {
            { ActivityType.SubmitBid, d => new SubmitBidTrophyEngine(d) }
        };

        public static ITrophyEngine CreateInstance(ActivityType activityType, IDataContextFactory dataContextFactor)
        {
            var engine = Dictionary[activityType];
            if (engine != null)
            {
                return engine.Invoke(dataContextFactor);
            }

            throw new NotImplementedException("Cannot find appropriate trophy engine");
        }
    }

    public interface ITrophyEngine
    {
        IEnumerable<Trophy> EvaluateActivityForPossibleTrophy(ApplicationUser user);
    }

    
}
