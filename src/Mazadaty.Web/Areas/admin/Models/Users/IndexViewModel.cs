using System.Collections.Generic;
using System.Web.Mvc;
using Mazadaty.Models;
using Mazadaty.Web.Core.Identity;

namespace Mazadaty.Web.Areas.admin.Models.Users
{
    public class IndexViewModel
    {
        public string Search { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        public SelectList RoleList { get; set; }
        public Role? Role { get; set; }
    }
}
