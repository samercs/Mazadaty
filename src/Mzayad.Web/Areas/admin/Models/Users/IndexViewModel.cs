using System.Collections.Generic;
using System.Web.Mvc;
using Mzayad.Models;

namespace Mzayad.Web.Areas.admin.Models.Users
{
    public class IndexViewModel
    {
        public string Search { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        public SelectList Roles { get; set; }
        public string Role { get; set; }
    }
}