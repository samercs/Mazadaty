using System.Collections.Generic;
using Mzayad.Models;

namespace Mzayad.Web.Areas.Admin.Models.Prizes
{
    public class IndexViewModel
    {
        public IEnumerable<Prize> Prizes { get; set; }
        public string Search { get; set; }
    }
}