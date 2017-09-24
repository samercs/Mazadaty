using System.Collections.Generic;
using Mazadaty.Models;

namespace Mazadaty.Web.Areas.Admin.Models.Prizes
{
    public class IndexViewModel
    {
        public IEnumerable<Prize> Prizes { get; set; }
        public string Search { get; set; }
    }
}
