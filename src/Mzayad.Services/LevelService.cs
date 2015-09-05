using Mzayad.Models;
using System;

namespace Mzayad.Services
{
    public class LevelService
    {
        public static Level GetLevel(int lvl)
        {
            if (lvl < 1)
            {
                return new Level()
                {
                    Name = "0",
                    Index = 0,
                    TokensAwarded = 0,
                    XpRequired = 0
                };
            }
            return new Level()
                    {
                        Name = lvl.ToString(),
                        Index = lvl,
                        TokensAwarded = 5 * lvl,
                        XpRequired = GetXp(lvl)
                    };
        }

        public static Level GetLevelByXp(int xp)
        {
            for (var i = 1;; i++)
            {
                var currLevel = GetLevel(i);
                var nextLevel = GetLevel(i + 1);

                if (nextLevel.XpRequired > xp)
                {
                    return currLevel;
                }
            }
        }

        private static int GetXp(int lvl)
        {
            return Level.XpBase/2*((int)Math.Pow(lvl, 2) - lvl);
        }
    }
}
