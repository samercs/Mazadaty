using Mzayad.Models;
using System.Collections.Generic;

namespace Mzayad.Web.Models.User
{
    public class FriendsViewModel
    {
        public IReadOnlyCollection<ApplicationUser> Friends { get; set; }
    }
}