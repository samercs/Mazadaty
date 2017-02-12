using Mzayad.Models;
using System.Collections.Generic;

namespace Mzayad.Web.Models.Prize
{
    public class SelectAvatarPrizeViewModel
    {
        public IEnumerable<Avatar> PremiumAvatars { get; set; }
        public int? SelectedAvatarId { get; set; }
    }
}