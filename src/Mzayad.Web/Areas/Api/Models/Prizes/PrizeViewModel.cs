using Humanizer;
using Mzayad.Models;

namespace Mzayad.Web.Areas.Api.Models.Prizes
{
    public class PrizeViewModel
    {
        public int PrizeId { get; set; }
        public string PrizeType { get; set; }
        public string Title { get; set; }
        public int? SubscriptionDays { get; set; }
        public decimal Weight { get; set; }

        public static PrizeViewModel Create(Prize prize)
        {
            return new PrizeViewModel
            {
                PrizeId = prize.PrizeId,
                Title = prize.Title,
                PrizeType = prize.PrizeType.Humanize(),
                SubscriptionDays = prize.SubscriptionDays,
                Weight = prize.Weight
            };
        }

    }

}

