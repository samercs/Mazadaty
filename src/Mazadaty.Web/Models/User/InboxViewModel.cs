using System.Collections.Generic;
using Mazadaty.Models;

namespace Mazadaty.Web.Models.User
{
    public class InboxViewModel
    {
        public IDictionary<ApplicationUser, Message> Messages { get; set; }
    }
}
