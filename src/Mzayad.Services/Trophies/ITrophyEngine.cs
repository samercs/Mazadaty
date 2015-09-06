using System.Collections.Generic;
using Mzayad.Models;
using Mzayad.Models.Enum;

namespace Mzayad.Services.Trophies
{
    public interface ITrophyEngine
    {
        IEnumerable<TrophyKey> GetEarnedTrophies(ApplicationUser user);
    }
}