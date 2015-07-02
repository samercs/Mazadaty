using Mzayad.Models;
using System;

namespace Mzayad.Services
{
    public class LevelService
    {
        public Level GetLevel(int lvl)
        {
            if (lvl < 1)
            {
                return new Level()
                {
                    Name = "0",
                    TokensAwarded = 0,
                    XpRequired = 0
                };
            }
            return new Level()
                    {
                        Name = lvl.ToString(),
                        TokensAwarded = 5 * lvl,
                        XpRequired = GetXp(lvl)
                    };
        }

        private static int GetXp(int lvl)
        {
            return Level.XpBase/2*((int)Math.Pow(lvl, 2) - lvl);
        }
    }
}
