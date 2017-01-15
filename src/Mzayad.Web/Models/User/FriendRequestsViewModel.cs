using Mzayad.Models;
using System.Collections.Generic;

namespace Mzayad.Web.Models.User
{
    public class FriendRequestsViewModel
    {
        public IReadOnlyCollection<FriendRequest> UserRequests { get; set; }

        public IReadOnlyCollection<FriendRequest> OthersRequests { get; set; }
    }
}