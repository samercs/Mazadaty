using System.Collections.Generic;
using Mzayad.Models;

namespace Mzayad.Web.Models.User
{
    public class InboxViewModel
    {
        public IDictionary<ApplicationUser, Message> Messages { get; set; }
    }
}