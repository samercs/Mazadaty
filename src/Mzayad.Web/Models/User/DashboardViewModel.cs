using System.Collections.Generic;
using Mzayad.Models;

namespace Mzayad.Web.Models.User
{
    public class DashboardViewModel
    {
        public ApplicationUser User { get; set; }
        public IReadOnlyCollection<Bid> BidHistory { get; set; } 
    }
}
