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
        public string UserId { get; set; }
        public string UserName { get; set; }
        public decimal BidAmount { get; set; }

        internal static BidModel Create(Bid bid)
        {
            return new BidModel
            {
                AvatarUrl = bid.User.AvatarUrl,
                UserId = bid.User.Id,
                UserName = bid.User.UserName,
                BidAmount = bid.Amount
            };
        }

        internal static LinkedList<BidModel> Create(IEnumerable<Bid> bids)
        {
            return new LinkedList<BidModel>(bids
                .OrderByDescending(i => i.BidId)
                .Take(3)
                .Select(Create));
        }
    }
}
