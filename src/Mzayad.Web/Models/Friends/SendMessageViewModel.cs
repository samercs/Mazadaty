using Mzayad.Models;
using System.Collections.Generic;

namespace Mzayad.Web.Models.Friends
{
    public class SendMessageViewModel
    {
        public Message Message { get; set; }
        public IReadOnlyCollection<Message> History { get; set; }
        public ApplicationUser User { get; set; }
    }
}