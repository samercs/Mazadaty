using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Mzayad.Models.Enum;
using Mzayad.Services;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace Mzayad.Web.Core.Services
{
    public abstract class TrophiesEngine
    {
        protected readonly TrophyService _trophyService;

        protected TrophiesEngine(TrophyService trophyService)
        {
            _trophyService = trophyService;
        }

        public async Task<bool> EarnTrophy(string userId, TrophyKey trophyKey)
        {
            return await _trophyService.AddToUser((int) trophyKey, userId);
        }

        public abstract Task<bool> CheckTrophy(string userId, TrophyKey trophy);
    }

    public class BidOnNewYearTrophy : TrophiesEngine
    {
        protected BidOnNewYearTrophy(TrophyService trophyService)
            :base(trophyService)
        {
        }

        public override async Task<bool> CheckTrophy(string userId, TrophyKey trophy)
        {
            if (DateTime.Now.Month != 1 || DateTime.Now.Day != 1)
            {
                return false;
            }
            var userTrophy = await _trophyService.GetLastEarnedTrophy(trophy, userId);
            return userTrophy.CreatedUtc.Year == DateTime.Now.Year;
        }
    }
}
