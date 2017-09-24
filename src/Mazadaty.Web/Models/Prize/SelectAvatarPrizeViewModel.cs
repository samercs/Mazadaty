using Mazadaty.Models;
using System.Collections.Generic;

namespace Mazadaty.Web.Models.Prize
{
    public class SelectAvatarPrizeViewModel
    {
        public IEnumerable<Avatar> PremiumAvatars { get; set; }
        public int? SelectedAvatarId { get; set; }
    }
}
