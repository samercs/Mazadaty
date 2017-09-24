using Mazadaty.Models;
using System.Collections.Generic;

namespace Mazadaty.Web.Models.User
{
    public class MyAvatarsViewModel
    {
        public ApplicationUser User { get; set; }
        public IEnumerable<Avatar> Avatars { get; set; }
        public int? SelectedAvatar { get; set; }
    }
}
