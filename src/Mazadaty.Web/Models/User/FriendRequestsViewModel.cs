using Mazadaty.Models;
using System.Collections.Generic;

namespace Mazadaty.Web.Models.User
{
    public class FriendRequestsViewModel
    {
        public IReadOnlyCollection<FriendRequest> OthersRequests { get; set; }
    }
}
