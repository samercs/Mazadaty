using System.Collections.Generic;
using System.Threading.Tasks;
using Mzayad.Models;
using Mzayad.Models.Enum;

namespace Mzayad.Services.Trophies
{
    public interface ITrophyEngine
    {
        Task<IEnumerable<TrophyKey>> TryGetEarnedTrophies(ApplicationUser user);
    }
}