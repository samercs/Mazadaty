using Mzayad.Data;
using Mzayad.Models.Enums;
using System;
using System.Collections.Generic;

namespace Mzayad.Services.Trophies
{
    public static class TrophyEngineFactory
    {
        private static readonly Dictionary<ActivityType, Func<IDataContextFactory, TrophyEngine>> Dictionary = new Dictionary<ActivityType, Func<IDataContextFactory, TrophyEngine>>
        {
            { ActivityType.SubmitBid, d => new SubmitBidTrophyEngine(d) },
            { ActivityType.VisitSite , d => new VisitSiteTrophyEngine(d)}
        };

        public static TrophyEngine CreateInstance(ActivityType activityType, IDataContextFactory dataContextFactor)
        {
            var engine = Dictionary[activityType];
            if (engine != null)
            {
                return engine.Invoke(dataContextFactor);
            }

            throw new NotImplementedException("Cannot find appropriate trophy engine");
        }
    }
}
