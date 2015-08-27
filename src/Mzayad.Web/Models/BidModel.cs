using Mzayad.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mzayad.Web.Models
{
    [Serializable]
    internal class BidModel
    {
        public string AvatarUrl { get; set; }
        public string UserName { get; set; }
        public decimal BidAmount { get; set; }

        internal static BidModel Create(Bid bid)
        {
            return new BidModel
            {
                AvatarUrl = bid.User.AvatarUrl,
                UserName = bid.User.UserName,
                BidAmount = bid.Amount
            };
        }

        internal static Queue<BidModel> Create(IEnumerable<Bid> bids)
        {
            return new Queue<BidModel>(bids
                .OrderByDescending(i => i.BidId)
                .Take(3)
                .OrderBy(i => i.BidId)
                .Select(Create));
        }
    }
}
