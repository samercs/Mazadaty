using System.Collections.Generic;
using System.Threading.Tasks;
using Mazadaty.Models;
using Mazadaty.Models.Enum;

namespace Mazadaty.Services.Trophies
{
    public interface ITrophyEngine
    {
        Task<IEnumerable<TrophyKey>> TryGetEarnedTrophies(ApplicationUser user);
    }
}
