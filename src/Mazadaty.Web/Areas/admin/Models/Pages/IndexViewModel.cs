using OrangeJetpack.Cms.Models;
using System.Collections.Generic;

namespace Mazadaty.Web.Areas.Admin.Models.Pages
{
    public class IndexViewModel
    {
        public IEnumerable<Page> Pages { get; set; }
    }
}
